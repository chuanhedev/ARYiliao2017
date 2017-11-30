using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;


public class HorizontalSlider : MonoBehaviour
{
	public bool draging = false;

	private float startDragX;
	private float startTransformX;
	private RectTransform transform;
	private int itemsCount;
	private float itemGap;
	private float minX;
	private float maxX;
	public bool enabled;
	[HideInInspector]
	public int index;
	public Action<int> onIndexChanged;
	public float behindItemScale = .9f;
	private List<GameObject> items;
	public List<GameObject> itemHeaders;
	public List<GameObject> itemFooters;

//	public void Start ()
//	{
//		transform = GetComponent<RectTransform> ();
//		maxX = transform.localPosition.x + itemGap;
//		minX = transform.localPosition.x - (itemsCount - 1) * (itemGap + itemWidth) - itemGap;
//	}

	public void Start(){
		transform = GetComponent<RectTransform> ();
		index = -1;
		items = new List<GameObject> ();
		for(int i =0; i<transform.childCount; i++)
			items.Add(transform.GetChild(i).gameObject);
		itemsCount = items.Count;
		itemGap = items [1].transform.localPosition.x - items [0].transform.localPosition.x;
		maxX = transform.localPosition.x + 50;
		minX = transform.localPosition.x - (itemsCount - 1) * itemGap - 50;
		enabled = true;
		Select (0);
	}

	public void SetItemDetails(int index, string title, string desc){
		GameObject item = items [index];
		GameObject t = item.GetChildByName("Title");
		GameObject d = item.GetChildByName("Desc");
		if (t)
			t.GetComponent<Text> ().text = title;
		if (d)
			d.GetComponent<Text> ().text = desc;
	}

	public void SliderBeginDrag(){
		draging = true;
		startDragX = Input.mousePosition.x;
		startTransformX = transform.localPosition.x;
		Debug.Log (Input.mousePosition.ToString());
	}

	public void SliderEndDrag(){
		draging = false;
		//int portion = Mathf.FloorToInt (-transform.localPosition.x * 2/ (itemGap + itemWidth));
		//int itemIdx = Mathf.FloorToInt ((portion + 1) / 2);
		//GetComponent<Transform>().D
		int tarIndex = index;
		float flipDistance = 100;
		if (transform.localPosition.x - startTransformX > flipDistance) {
			tarIndex = Mathf.Max (index - 1, 0);
		}else if (transform.localPosition.x - startTransformX < -flipDistance) {
			tarIndex = Mathf.Min (index + 1, itemsCount - 1);
		}

		transform.DOLocalMoveX(-tarIndex * (itemGap), .1f).OnComplete(
			()=>Select(tarIndex)
		);
//		if (origIdx != index && onIndexChanged != null)
//			onIndexChanged.Invoke (index);
		//Debug.Log (transform.localPosition.x + " " + startTransformX  + " " + index + " SliderEndDrag");
	}

	public void Select(int idx){
		Debug.Log ("Select " + idx);
		if (idx == index)
			return;
		index = idx;
		for (int i = 0; i < itemsCount; i++) {
			if(i==index){
				itemHeaders [i].SetAlpha (1);
				itemFooters [i].transform.GetChild (1).gameObject.SetActive (false);
			}else{
				itemHeaders [i].SetAlpha (0);
				itemFooters [i].transform.GetChild (1).gameObject.SetActive (true);
			}
		}
		if (onIndexChanged != null)
			onIndexChanged.Invoke (index);
		
	}

	public void SliderDrag(){
		//Debug.Log ("SliderDrag");
	}


	void Update(){
		for (int i = 0; i < itemsCount; i++) {
			items [i].transform.localScale = (1 - 0.1f * Mathf.Abs (items [i].transform.localPosition.x + transform.localPosition.x) / itemGap) * Vector3.one;
			itemHeaders [i].SetAlpha (1- Mathf.Abs (items [i].transform.localPosition.x + transform.localPosition.x) / itemGap);
		}

		if (!enabled)
			return;
		if (draging) {
			transform.localPosition = transform.localPosition.SetX(Mathf.Clamp( startTransformX + Input.mousePosition.x - startDragX , minX, maxX ) );  
		}
	}
}
