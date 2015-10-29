using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {

	public GameObject[] levels;
	public GameObject[] menus;
	public Camera mainCamera;

	// Use this for initialization
	void Start()
	{
		DisplayMenu();
	}

	public void HideLevelsNMenus()
	{
		for (int i=0; i<levels.Length; ++i)
			levels[i].SetActive(false);
		for (int i=0; i<menus.Length; ++i)
			menus[i].SetActive(false);
	}

	public void DisplayMenu(int menu=0)
	{
		HideLevelsNMenus();
		mainCamera.gameObject.SetActive(true);
		menus[menu].SetActive(true);
	}

	public void StartLevel(int level=0)
	{
		HideLevelsNMenus();
		mainCamera.gameObject.SetActive(false);
		levels[level].SetActive(true);
		levels[level].DetachChildren();
	}
}
