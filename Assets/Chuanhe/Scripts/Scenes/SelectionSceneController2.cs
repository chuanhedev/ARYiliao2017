using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Xml.Linq;
using System.IO;
using UnityEngine.UI;

public class SelectionSceneController2 : MonoBehaviour
{
	//public GameObject selectionItem;
	public HorizontalSlider slider;
	//public GameObject itemsPanel;
	public ProgressPanel progressPanel;
	public OKCancelPanel okCancelPanel;
	private XElement layout;
	//private List<GameObject> selectionItems;
	[HideInInspector]
	public Text phone;
	[HideInInspector]
	public Text email;
	[HideInInspector]
	public Text website;
	public GameObject header;
	// Use this for initialization
	private bool enabled = true;
	private ConfigLoader configLoader;
	public GameObject infoPanel;

	public void ShowInfoPanel(){
		infoPanel.SetActive (true);
	}

	public void HideInfoPanel(){
		infoPanel.SetActive (false);
	}

	void Start ()
	{
		progressPanel.onCancelHandler = () => {
			configLoader.Cancel();
			progressPanel.Hide();
		};
		phone = infoPanel.GetChildByName ("Phone").GetComponent<Text> ();
		email = infoPanel.GetChildByName ("Email").GetComponent<Text> ();
		website = infoPanel.GetChildByName ("Website").GetComponent<Text> ();
		string txtPhone = I18n.Translate("select_phone");
		string txtEmail = I18n.Translate("select_email");
		string txtWebsite = I18n.Translate("select_website");
		if(!string.IsNullOrEmpty(txtPhone)) phone.text = txtPhone;
		if(!string.IsNullOrEmpty(txtEmail)) email.text = txtEmail;
		if(!string.IsNullOrEmpty(txtWebsite)) website.text = txtWebsite;
		GameObject version = infoPanel.GetChildByName ("Version");
		if (version != null)
			version.GetComponent<Text> ().text = "v" + Director.version.ToString ();
		StartCoroutine (initScene ());
	}

	IEnumerator initScene ()
	{
		yield return Request.ReadPersistent ("ui/ui.xml", LayoutLoaded);
		if (layout != null) {
//			XElement itemsEle = layout.Element ("items");
//			var items = itemsEle.Elements ();
//			int index = 0;
//			foreach (XElement item in items) {
//				string desc = Xml.Attribute (item, "desc");
//				string title = Xml.Attribute (item, "title");
//				string help = Xml.Attribute (item, "help");
//				string icon = Xml.Attribute (item, "icon");
//				GameObject obj = GameObject.Instantiate (selectionItem);
//				//obj.transform.r
//				//obj.transform.parent = itemsPanel.gameObject.transform;
//				//obj.GetComponent<RectTransform> ().localPosition = Vector3.zero;
//				obj.transform.SetParent(itemsPanel.transform, false);
//				RectTransform rectT = obj.GetComponent<RectTransform> ();
//				rectT.localPosition = new Vector3 (rectT.localPosition.x, rectT.localPosition.y-168 * index);
//				selectionItems.Add (obj);
//
//				SelectionItem itemComp = obj.GetComponent<SelectionItem> ();
//				itemComp.name = title;
//				itemComp.type = Xml.Attribute (item, "type");
//				itemComp.title.text = I18n.Translate (title);
//				itemComp.description.text = I18n.Translate (desc);
//				itemComp.btnInfo.SetActive (false);// (!string.IsNullOrEmpty (help));
//				itemComp.helpLink = Request.RemoteUrl + help;
//				itemComp.SetOnClick (OnItemClick);
////				WWW www = new WWW(Path.Combine(Application.persistentDataPath, "ui/"+icon));
////				itemComp.image.sprite = Sprite.Create(www.texture, new Rect(0,0,www.texture.width, www.texture.height), new Vector2(0,0));
//				StartCoroutine(LoadIcon ("ui/"+icon, itemComp.image));
//				index++;
//			}
		}
	}

	bool Enabled{
		get{
			return enabled;
		}
		set{
			enabled = value;
//			if (enabled) {
//				for (int i = 0; i < selectionItems.Count; i++) {
//					Logger.Log ("enbaled ", "red");
//					Button btn = selectionItems [i].GetComponent<Button> ();
//					btn.interactable = enabled;
//				}
//			}
		}
	}

	public void OnItemClick(){
		//item.gameObject.GetComponent<Button> ().interactable = false;
		StartCoroutine (OnItemClickHandler ());
	}

	IEnumerator OnItemClickHandler(){
		int idx = slider.index;
		Logger.Log ("clicked " + idx);
		okCancelPanel.Reset ();
		Enabled = false;
		configLoader = new ConfigLoader ();
		//configLoader.loadedHandler = FileLoaded;
		configLoader.progressHandler = FileProgressing;
		configLoader.okCancelPanel = okCancelPanel;
		XElement itemsEle = layout.Element ("items");
		XElement item = Xml.GetChild (itemsEle, idx);
		string type = Xml.Attribute (item, "type");
		string name = Xml.Attribute (item, "title");
		yield return configLoader.LoadConfig (name + "/config.xml");
		progressPanel.Hide ();
		Enabled = true;
		if (!configLoader.forceBreak && !okCancelPanel.isCancel) {
			Hashtable arg = new Hashtable ();
			arg.Add ("type", type);
			arg.Add ("name", name);
			arg.Add ("data", Xml.GetChildByAttribute(itemsEle, "title", name));
			SceneManagerExtension.LoadScene ("Scan", arg);
		}
	}

	void FileProgressing(int idx, int total, float progress){
		progressPanel.fileSize = configLoader.fileSize;
		progressPanel.Show (idx, total, progress);
	}

//	void FileLoaded(int idx, int total){
//		if (idx == 0) {
//			progressPanel.Show (total);
//			return;
//		}
//		progressPanel.fileSize = configLoader.fileSize;
//		progressPanel.Load (idx);
//		if (idx == total) {
//			progressPanel.Hide ();
//		}
//	}

	IEnumerator LoadIcon(string url, Image image){
		//Debug.Log (Path.Combine ("file:////"+ Application.persistentDataPath, url));
		WWW www = new WWW(Request.ResolvePath(Application.persistentDataPath + "/" + url));
		yield return www;
		image.sprite = Sprite.Create(www.texture, new Rect(0,0,www.texture.width, www.texture.height), new Vector2(0,0));
	}
//	void OnGUI ()
//	{
////		for (int i = 0; i < selectionItems.Count; i++) {
////			selectionItems [i].GetComponent<RectTransform> ().localPosition = Vector3.zero;
////		}
//	}

	void LayoutLoaded (string str)
	{
		layout = XDocument.Parse (str).Root;
	}

	public void OnEmailClick(){
		Application.OpenURL ("mailto://" + email.text);
	}

	public void OnPhoneClick(){
		Application.OpenURL ("tel://" + phone.text);
	}

	public void OnWebsiteClick(){
		Application.OpenURL (website.text);
	}
}
