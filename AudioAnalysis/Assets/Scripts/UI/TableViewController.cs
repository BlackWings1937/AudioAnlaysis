using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class TableViewController<T> : ViewController		// ViewControllerクラスを継承
{
	protected List<T> tableData = new List<T>();			// リスト項目のデータを保持
	[SerializeField] protected RectOffset padding;			// スクロールさせる内容のパディング
	[SerializeField] private float spacingHeight = 4.0f;	// 各セルの間隔
    [SerializeField] private float spacingWidth = 4.0f;

	// Scroll Rectコンポーネントをキャッシュ
	private ScrollRect cachedScrollRect;
	public ScrollRect CachedScrollRect
	{
		get {
			if(cachedScrollRect == null) { 
				cachedScrollRect = GetComponent<ScrollRect>(); }
			return cachedScrollRect;
		}
	}

    public void RemoveAllCellTexture() {
        LinkedListNode<TableViewCell<T>> node = cells.First;
        while (node != null)
        {
            node.Value.RemoveTexture();
            node = node.Next;
        }
    }

    public void Clear() {
        tableData = null;//.Clear();
        LinkedListNode<TableViewCell<T>> node = cells.First;//mark cell index
        //List<GameObject> listNeedBeDestroyed = new List<GameObject>();
        while(node != null){
            GameObject.Destroy(node.Value.gameObject);
            node = node.Next;
        }
        cells.Clear();

        ((RectTransform)CachedScrollRect.content).sizeDelta = CachedRectTransform.sizeDelta;
        Vector3 p = ((RectTransform)CachedScrollRect.content).localPosition;
        p.x = 0;// -CachedRectTransform.sizeDelta.x / 2;
        ((RectTransform)CachedScrollRect.content).localPosition = p;
    }

	// インスタンスのロード時に呼ばれる
	protected virtual void Awake()
	{
	}
	
	// リスト項目に対応するセルの高さを返すメソッド
	protected virtual float CellHeightAtIndex(int index)
	{
		// 実際の値を返す処理は継承したクラスで実装する
		return 0.0f;
	}

    // 计算cell 宽度，在某一索引下
    protected virtual float CellWidthAtIndex(int index) {
        return 0.0f;
    }

	// スクロールさせる内容全体の高さを更新するメソッド
	protected void UpdateContentSize()
	{
        /*
         * 判断滚动层为横向滑动还是纵向滑动
         */
        if(CachedScrollRect.horizontal){
            float contentWidth = 0.0f;
            for (int i = 0; i < tableData.Count;++i )
            {
                contentWidth += CellWidthAtIndex(i);
                if (i > 0) { contentWidth += spacingWidth; }
            }
            Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
            sizeDelta.x = padding.left + padding.right + contentWidth;
            CachedScrollRect.content.sizeDelta = sizeDelta;
        }else{
            // スクロールさせる内容全体の高さを算出する
            float contentHeight = 0.0f;
            for (int i = 0; i < tableData.Count; ++i)
            {
                contentHeight += CellHeightAtIndex(i);
                if (i > 0) { contentHeight += spacingHeight; }
            }

            // スクロールさせる内容の高さを設定する
            Vector2 sizeDelta = CachedScrollRect.content.sizeDelta;
            sizeDelta.y = padding.top + contentHeight + padding.bottom;
            CachedScrollRect.content.sizeDelta = sizeDelta;
        }
	}

#region セルを作成するメソッドとセルの内容を更新するメソッドの実装
	[SerializeField] private GameObject cellBase;	// コピー元のセル
	protected LinkedList<TableViewCell<T>> cells = 
		new LinkedList<TableViewCell<T>>();			// セルを保持

	// インスタンスのロード時Awakeメソッドの後に呼ばれる
	protected virtual void Start()
	{
		// コピー元のセルは非アクティブにしておく
		cellBase.SetActive(false);

#region セルを再利用する処理の実装
		// Scroll RectコンポーネントのOn Value Changedイベントのイベントリスナーを設定する
		CachedScrollRect.onValueChanged.AddListener(OnScrollPosChanged);
#endregion
	}

	// セルを作成するメソッド
	private TableViewCell<T> CreateCellForIndex(int index)
	{
		// コピー元のセルから新しいセルを作成する
		GameObject obj = Instantiate(cellBase) as GameObject;
		obj.SetActive(true);
		TableViewCell<T> cell = obj.GetComponent<TableViewCell<T>>();
		cell.transform.SetParent(CachedScrollRect.content.transform,false);

		UpdateCellForIndex(cell, index);

		cells.AddLast(cell);

		return cell;
	}

    public virtual void RefreshCell() {
 
    }

	// セルの内容を更新するメソッド
	protected void UpdateCellForIndex(TableViewCell<T> cell, int index)
	{
		// セルに対応するリスト項目のインデックスを設定する
		cell.DataIndex = index;

		if(cell.DataIndex >= 0 && cell.DataIndex <= tableData.Count-1)
		{
			// セルに対応するリスト項目があれば、セルをアクティブにして内容を更新し、高さを設定する
			cell.gameObject.SetActive(true);
			cell.UpdateContent(tableData[cell.DataIndex]);
			cell.Height = CellHeightAtIndex(cell.DataIndex);
		}
		else
		{
			// セルに対応するリスト項目がなかったら、セルを非アクティブにして表示しない
			cell.gameObject.SetActive(false);
		}
	}
#endregion

#region visibleRectの定義とvisibleRectを更新するメソッドの実装
	public Rect visibleRect;								// リスト項目をセルとして表示する範囲を示す矩形
	[SerializeField] private RectOffset visibleRectPadding;	// visibleRectのパディング

	// visibleRectを更新するためのメソッド
	protected void UpdateVisibleRect()
	{
		// visibleRectの位置はスクロールさせる内容の基準点からの相対位置
		visibleRect.x = 
			-CachedScrollRect.content.anchoredPosition.x + visibleRectPadding.left;
		visibleRect.y = 
			-CachedScrollRect.content.anchoredPosition.y + visibleRectPadding.top;

		// visibleRectのサイズはスクロールビューのサイズ+パディング
        visibleRect.width = CachedRectTransform.rect.width + 
			visibleRectPadding.left + visibleRectPadding.right;
        visibleRect.height =  CachedRectTransform.rect.height + 
			visibleRectPadding.top + visibleRectPadding.bottom;
	}
#endregion

#region テーブルビューの表示内容を更新する処理の実装

    protected void UpdateContentsByIndex(int index) {

        UpdateContentSize();

        Vector2 sizeOfContent = ((RectTransform)CachedScrollRect.content.transform).sizeDelta;
        Vector2 sizeOfScroll = CachedRectTransform.sizeDelta;
        float min = -(sizeOfContent.x - sizeOfScroll.x);
        float max = 0;

        float contentX = (index ) * -CellWidthAtIndex(index) + (index - 1)*-spacingWidth;
        if (contentX > max) contentX = max;
        if (contentX < min) contentX = min;

        Vector3 posContent = cachedScrollRect.content.transform.localPosition;
        posContent.x = contentX;
        cachedScrollRect.content.transform.localPosition = posContent;

        UpdateVisibleRect();	// visibleRectを更新する

        if (cells.Count < 1)
        {
            /*
             *  查找添加第一个在可视返回内的item
             */
            if (CachedScrollRect.horizontal)
            {
                Vector2 cellLeft = new Vector2(padding.left, 0.0f);
                for (int i = 0; i < tableData.Count; ++i)
                {
                    float cellWidth = CellWidthAtIndex(i);
                    Vector2 cellRight = cellLeft + new Vector2(cellWidth, 0.0f);
                    if ((cellLeft.x >= visibleRect.x &&
                        cellLeft.x <= visibleRect.x + visibleRect.width) ||// mark visible rect
                        (cellRight.x >= visibleRect.x &&
                        cellRight.x <= visibleRect.x + visibleRect.width))
                    {
                        TableViewCell<T> cell = CreateCellForIndex(i);
                        cell.Left = cellLeft;
                        break;
                    }
                    cellLeft = cellRight + new Vector2(spacingWidth, 0.0f);
                }
            }
            else
            {
                Vector2 cellTop = new Vector2(0.0f, -padding.top);
                for (int i = 0; i < tableData.Count; ++i)
                {
                    float cellHeight = CellHeightAtIndex(i);
                    Vector2 cellBottom = cellTop + new Vector2(0.0f, -cellHeight);
                    if ((cellTop.y <= visibleRect.y &&
                        cellTop.y >= visibleRect.y - visibleRect.height) ||
                       (cellBottom.y <= visibleRect.y &&
                        cellBottom.y >= visibleRect.y - visibleRect.height))
                    {
                        TableViewCell<T> cell = CreateCellForIndex(i);
                        cell.Top = cellTop;
                        break;
                    }
                    cellTop = cellBottom + new Vector2(0.0f, spacingHeight);
                }

            }
            // visibleRectの範囲内に空きがあればセルを作成する
            FillVisibleRectWithCells();
        }
        else
        {
            cells.First.Value.DataIndex = index-1;
            float cellWidth = CellWidthAtIndex(index);
            Vector2 cellLeft = new Vector2(padding.left +( index  - 1)* cellWidth + (index - 2) * spacingWidth, 0.0f);
            cells.First.Value.Left = cellLeft;
            LinkedListNode<TableViewCell<T>> node = cells.First;//mark cell index
            UpdateCellForIndex(node.Value, node.Value.DataIndex);
            node = node.Next;

            while (node != null)
            {
                UpdateCellForIndex(node.Value, node.Previous.Value.DataIndex + 1);
                if (CachedScrollRect.horizontal)
                {
                    node.Value.Left =
                        node.Previous.Value.Right + new Vector2(spacingWidth, 0.0f);
                }
                else
                {
                    node.Value.Top =
                        node.Previous.Value.Bottom + new Vector2(0.0f, -spacingHeight);
                }
                node = node.Next;
            }

            // visibleRectの範囲内に空きがあればセルを作成する
            FillVisibleRectWithCells();
        }
        OnScrollPosChanged(CachedScrollRect.content.position);
    }

	protected void UpdateContents()
	{
		UpdateContentSize();	// スクロールさせる内容のサイズを更新する
        /*
         * mark 移动content到目标位置
         */
		UpdateVisibleRect();	// visibleRectを更新する

		if(cells.Count < 1)
		{
			/*
             *  查找添加第一个在可视返回内的item
             */
            if(CachedScrollRect.horizontal){
                Vector2 cellLeft = new Vector2(padding.left,0.0f);
                for (int i = 0; i < tableData.Count;++i )
                {
                    float cellWidth = CellWidthAtIndex(i);
                    Vector2 cellRight = cellLeft + new Vector2(cellWidth,0.0f);
                    if((cellLeft.x>= visibleRect.x &&
                        cellLeft.x<=visibleRect.x + visibleRect.width)||// mark visible rect
                        (cellRight.x>=visibleRect.x &&
                        cellRight.x<=visibleRect.x + visibleRect.width)){
                            TableViewCell<T> cell = CreateCellForIndex(i);
                            cell.Left = cellLeft;
                            break;
                    }
                    cellLeft = cellRight + new Vector2(spacingWidth,0.0f);
                }
            }else{
                Vector2 cellTop = new Vector2(0.0f, -padding.top);
                for (int i = 0; i < tableData.Count; ++i)
                {
                    float cellHeight = CellHeightAtIndex(i);
                    Vector2 cellBottom = cellTop + new Vector2(0.0f, -cellHeight);
                    if ((cellTop.y <= visibleRect.y &&
                        cellTop.y >= visibleRect.y - visibleRect.height) ||
                       (cellBottom.y <= visibleRect.y &&
                        cellBottom.y >= visibleRect.y - visibleRect.height))
                    {
                        TableViewCell<T> cell = CreateCellForIndex(i);
                        cell.Top = cellTop;
                        break;
                    }
                    cellTop = cellBottom + new Vector2(0.0f, spacingHeight);
                }
                 
            }
			// visibleRectの範囲内に空きがあればセルを作成する
			FillVisibleRectWithCells();
		}
		else
		{
			LinkedListNode<TableViewCell<T>> node = cells.First;//mark cell index
			UpdateCellForIndex(node.Value, node.Value.DataIndex);
			node = node.Next;
			
			while(node != null)
			{
                UpdateCellForIndex(node.Value, node.Previous.Value.DataIndex + 1);
                if(CachedScrollRect.horizontal){
                    node.Value.Left =
                        node.Previous.Value.Right + new Vector2(spacingWidth,0.0f);
                }else{
                    node.Value.Top =
                        node.Previous.Value.Bottom + new Vector2(0.0f, -spacingHeight);
                }
                node = node.Next;
			}

			// visibleRectの範囲内に空きがあればセルを作成する
			FillVisibleRectWithCells();
		}
        OnScrollPosChanged(CachedScrollRect.content.position);
    }

	// visibleRectの範囲内に表示される分のセルを作成するメソッド
	private void FillVisibleRectWithCells()
	{
		// セルがなければ何もしない
		if(cells.Count < 1)
		{
			return;
		}

	    /*
         * 填充完剩余cell
         */
        if(CachedScrollRect.horizontal){
            TableViewCell<T> lastCell = cells.Last.Value;
            int nextCellDataIndex = lastCell.DataIndex + 1;
            Vector2 nextCellLeft = lastCell.Right + new Vector2(spacingWidth,0.0f);
            while(nextCellDataIndex<tableData.Count&&nextCellLeft.x<=visibleRect.x+visibleRect.width){
                TableViewCell<T> cell = CreateCellForIndex(nextCellDataIndex);
                cell.Left = nextCellLeft;

                lastCell = cell;
                nextCellDataIndex = lastCell.DataIndex + 1;
                nextCellLeft = lastCell.Right + new Vector2(spacingWidth,0.0f);
            }
        }else{
            TableViewCell<T> lastCell = cells.Last.Value;
            int nextCellDataIndex = lastCell.DataIndex + 1;
            Vector2 nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

            while (nextCellDataIndex < tableData.Count &&
                nextCellTop.y >= visibleRect.y - visibleRect.height)
            {
                TableViewCell<T> cell = CreateCellForIndex(nextCellDataIndex);
                cell.Top = nextCellTop;

                lastCell = cell;
                nextCellDataIndex = lastCell.DataIndex + 1;
                nextCellTop = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);
            }
        }
		
	}
#endregion

#region セルを再利用する処理の実装
	private Vector2 prevScrollPos;	// 前回のスクロール位置を保持

	// スクロールビューがスクロールされたときに呼ばれる
	public void OnScrollPosChanged(Vector2 scrollPos)
	{
		// visibleRectを更新する
		UpdateVisibleRect();
		// スクロールした方向によって、セルを再利用して表示を更新する
        if(CachedScrollRect.horizontal){
            ReuseCells((scrollPos.x < prevScrollPos.x) ? -1 : 1);
        }else{
            ReuseCells((scrollPos.y < prevScrollPos.y) ? 1 : -1);
        }

		prevScrollPos = scrollPos;
	}

	// セルを再利用して表示を更新するメソッド
	private void ReuseCells(int scrollDirection)
	{
		if(cells.Count < 1)
		{
			return;
		}
        
        if(CachedScrollRect.horizontal){
            if(scrollDirection>0){
                TableViewCell<T> firstCell = cells.First.Value;
                while(firstCell.Right.x<visibleRect.x){
                    TableViewCell<T> lastCell = cells.Last.Value;
                    UpdateCellForIndex(firstCell,lastCell.DataIndex + 1);
                    firstCell.Left = lastCell.Right + new Vector2(spacingWidth,0.0f);

                    cells.AddLast(firstCell);
                    cells.RemoveFirst();
                    firstCell = cells.First.Value;
                }
                FillVisibleRectWithCells();
            }else if(scrollDirection<0){
                TableViewCell<T> lastCell = cells.Last.Value;
                while(lastCell.Left.x>visibleRect.x +visibleRect.width){
                    TableViewCell<T> firstCell = cells.First.Value;
                    UpdateCellForIndex(lastCell,firstCell.DataIndex -1);
                    lastCell.Right = firstCell.Left + new Vector2(-spacingWidth,0.0f);

                    cells.AddFirst(lastCell);
                    cells.RemoveLast();
                    lastCell = cells.Last.Value;
                }
            }
        }else{
            if (scrollDirection > 0)
            {
                // 上にスクロールしている場合、visibleRectの範囲より上にあるセルを
                // 順に下に移動して内容を更新する
                TableViewCell<T> firstCell = cells.First.Value;
                while (firstCell.Bottom.y > visibleRect.y)
                {
                    TableViewCell<T> lastCell = cells.Last.Value;
                    UpdateCellForIndex(firstCell, lastCell.DataIndex + 1);
                    firstCell.Top = lastCell.Bottom + new Vector2(0.0f, -spacingHeight);

                    cells.AddLast(firstCell);
                    cells.RemoveFirst();
                    firstCell = cells.First.Value;
                }

                // visibleRectの範囲内に空きがあればセルを作成する
                FillVisibleRectWithCells();
            }
            else if (scrollDirection < 0)
            {
                // 下にスクロールしている場合、visibleRectの範囲より下にあるセルを
                // 順に上に移動して内容を更新する
                TableViewCell<T> lastCell = cells.Last.Value;
                while (lastCell.Top.y < visibleRect.y - visibleRect.height)
                {
                    TableViewCell<T> firstCell = cells.First.Value;
                    UpdateCellForIndex(lastCell, firstCell.DataIndex - 1);
                    lastCell.Bottom = firstCell.Top + new Vector2(0.0f, spacingHeight);

                    cells.AddFirst(lastCell);
                    cells.RemoveLast();
                    lastCell = cells.Last.Value;
                }
            }
        }

		
	}
#endregion
}
