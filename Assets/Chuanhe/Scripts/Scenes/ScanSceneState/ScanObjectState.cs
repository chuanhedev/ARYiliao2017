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

public class ScanObjectState:ScanSceneState
{
	private AudioSource audio;
	private Vector3 localScale;
	private Quaternion localRotation;
	private Transform model;
	private float fullscreenScale;
	private Transform parent;
	//private Vector3 localScale;

	public ScanObjectState(){
		name = "object";
	}

	public override void OnEnter(Hashtable args = null){
		base.OnEnter ();
		fullscreenScale = Xml.Float (scene.GetCurrentItemInfo (), "fullscreenScale", 1);
		audio = ScanSceneController.currentTrackableObject.GetComponent<AudioSource> ();
		TrackStart (ScanSceneController.currentTrackableObject.name, "Object");
		if (audio)
			audio.Play ();
		GameObject modelObj = ScanSceneController.currentTrackableObject.GetChildByName("model");
		if (modelObj) {
			modelObj.SetActive (true);
			model = modelObj.transform;
			localScale = model.localScale;
			model.localRotation = Quaternion.identity;
			parent = model.transform.parent;
		}
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
		model.transform.SetParent(scene.ARCamera.GetChildByName ("ObjectContainer").transform, false);
		model.transform.localScale = model.transform.localScale * fullscreenScale;
	}

	public override void OnExit(){
		model.gameObject.SetActive (false);
		model.transform.SetParent(parent, false);
		model.transform.localScale = model.transform.localScale / fullscreenScale;
		TrackEnd (ScanSceneController.currentTrackableObject.name, "Object");
		if (audio)
			audio.Stop ();
		if (model != null && localScale!=null) {
			model.localScale = localScale;
			//firstChild.localRotation = localRotation;
		}
		base.OnExit ();
	}
}
