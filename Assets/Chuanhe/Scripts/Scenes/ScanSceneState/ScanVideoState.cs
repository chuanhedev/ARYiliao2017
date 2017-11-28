 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.Networking;
using Vuforia;
using UnityEngine.SceneManagement;
using System.Xml.Linq;
using UnityEngine.Video;

public class ScanVideoState:ScanSceneState{
    //private VideoPlayer vp;
	protected GameObject videoContainer;
	protected GameObject videoPanel;

    public ScanVideoState(){
		name = "video";
	}
		
	public override void OnEnter (Hashtable args = null)
	{
		base.OnEnter(args);
		videoContainer = ScanSceneController.currentTrackableObject.transform.GetChild(0).gameObject;
		videoContainer.SetActive (true);
		videoPanel = videoContainer.transform.GetChild(0).gameObject;
        string path = ScanSceneController.currentTrackableObject.GetComponent<CustomTrackableEventHandler>().videoPath;
        Logger.Log(path, "purple");
		scene.ShowVideoSlide ();
		RegisterClick (videoContainer);
		VideoController.instant.Play(videoPanel, path);
		TrackStart (ScanSceneController.currentTrackableObject.name, "Video");
		scene.topRightButton.ShowChildByName ("Fullscreen");
		SetTitle ();
    }

	public override void OnInfoClick ()
	{
		FullScreen ();
	}

	public void FullScreen(){
		isFullScreen = true;
		scene.topRightButton.ShowChildByName ("null");
		Transform trans = videoPanel.GetComponent<Transform>();
		videoContainer.SetActive (false);
		//videoPlayer.Play ();
		scene.videoPanel.videoPlayer.GetComponent<RectTransform>().localScale = new Vector2(1, Mathf.Min(1.4f, trans.lossyScale.z/trans.lossyScale.x));
		//VideoPlayer videoPlayer = ;
		scene.videoPanel.Show();
		scene.videoPanel.Play(VideoController.instant.videoPlayer);
	}

	protected void RegisterClick(GameObject obj, string nextState="idle"){
		if (Configuration.instant.enablePopupVideo) {
			OnClick click = obj.GetComponent<OnClick> ();
			if (click != null) {
				Logger.Log ("RegisterClick");
				click.OnClickHandler = (o) => this.FullScreen();
			}
		}
	}




//	public override void OnEnter (Hashtable args = null)
//	{
//		//videoPlayer.targetMaterialRenderer = ScanSceneController.instant.videoPanel.GetComponent<MeshRenderer>();
//		//o.SetActive(false);
//		GameObject obj = args["object"] as GameObject ;
//		nextState = args ["nextstate"].ToString ();
//		Transform trans = obj.GetComponent<Transform>();
//		obj.SetActive (false);
//		scene.videoPanel.SetActive(true);
//		RawImage image= scene.videoPanel.GetComponent<RawImage>();
//		VideoPlayer videoPlayer = VideoController.instant.videoPlayer;
//		videoPlayer.renderMode = VideoRenderMode.RenderTexture;
//		videoPlayer.Play ();
//		scene.videoPanel.GetComponent<RectTransform>().localScale = new Vector2(1, Mathf.Min(1.4f, trans.localScale.z/trans.localScale.x));
//		image.texture = videoPlayer.texture;
//	}

	public override void OnBackClick(){
		if (scene.videoPanel.shown) {
			scene.videoPanel.Hide();
			videoContainer.SetActive (false);
			ScanSceneController.instant.SetState ("idle");
		}else
			base.OnBackClick ();
	}


	public override void OnExit(){
		base.OnExit ();
		TrackEnd (ScanSceneController.currentTrackableObject.name, "Video");
		VideoController.instant.Stop ();
    }
}
