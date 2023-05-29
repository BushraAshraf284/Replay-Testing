using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [Header("UI References")]
    public GameObject Menu;
    public GameObject SubMenu;

    [SerializeField]
    private PlayerController[] playerController;

    private void Awake()
    {
        playerController = FindObjectsOfType<PlayerController>();
        for(int i =0; i < playerController.Length; i++)
        {
            playerController[i].index = i;
            playerController[i].SetFilePaths();
        }
    }

    private void Start()
    {
        Menu.SetActive(true);
        SubMenu.SetActive(false);
    }

    public void SwitchMenu()
    {
        Menu.SetActive(false);
        SubMenu.SetActive(true);
    }
    
    public void ChooseDataType(int i)
    {
        PlayerPrefs.SetInt("Class", i);
        RecordGamePlay();
    }
    public void RecordGamePlay()
    {
        SubMenu.SetActive(false);
        ManagePlayers(Action.Record);
      //  playerController.StartRecording();
    }

    public void ReplayGamePlay()
    {
        Menu.SetActive(false);
        ManagePlayers(Action.Replay);
    }

    public void Exit()
    {
        Menu.SetActive(true);
        ManagePlayers(Action.EndRecord);
    }

    public void ManagePlayers(Action action)
    {
        for(int i =0; i < playerController.Length;i++)
        {
            switch (action)
            {
                case Action.Record:
                    playerController[i].StartRecording();
                    break;
                case Action.Replay:
                    playerController[i].Replay(); 
                    break;
                case Action.EndRecord:
                    playerController[i].EndRecording();
                    break;
            }
        }
      
    }


}

public enum Action
{
    Record,
    Replay,
    EndRecord
}
