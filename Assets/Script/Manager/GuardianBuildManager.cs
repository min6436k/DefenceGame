using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GuardianBuildManager : MonoBehaviour
{
    public GameObject[] Tiles;

    public GameObject CurrentFocusTile; //현재 선택된 타일
    public GameObject GuardianPrefab;
    public GameObject BuildIconPrefab;

    public Material BuildCanMat; //건축할 수 있을 때의 마테리얼
    public Material BuildCanNotMat; //건축할 수 없을 때의 마테리얼

    public float BuildDeltaY = 0f; //건축 시 가디언의 y좌표에 더할 값
    public float FocusTileDistance = 0.05f; //포커스로 인식할 거리

    public int NormalGuaridanCost = 50; //가디언의 가격

    public UnityEvent OnBuild;

    void Start()
    {
        Tiles = GameObject.FindGameObjectsWithTag("Tile"); //씬에 있는 Tile 태그를 가진 오브젝트들을 배열에 할당
        BuildIconPrefab = Instantiate(BuildIconPrefab, transform.position, Quaternion.Euler(90, 0, 0)); //빌드 아이콘 프리팹을 생성
        BuildIconPrefab.gameObject.SetActive(false); //생성한 프리팹을 비활성화
    }

    void Update()
    {
        bool bisUpgrading = GameManager.Inst.guardianUpgradeManager.bIsUpgrading; //guardianUpgradeManager에서 업그레드가 가능한지의 변수 할당

        UpdateFindFocusTile();
        if (!bisUpgrading)
        {
            UpdateBuildImage();
            UpdateKeyInput();
        }
    }

    private void UpdateFindFocusTile()
    {
        CurrentFocusTile = null; //현재 포커스중인 타일 null로 초기화
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));

        RaycastHit hit;
        if (Physics.Raycast(mousePosition, Camera.main.transform.forward, out hit, 500,LayerMask.GetMask("Tile")))
        {
            if (hit.collider.CompareTag("Tile")) CurrentFocusTile = hit.collider.gameObject;
        }

        //mousePosition변수에 마우스포인터의 위치를 월드 좌표로 바꾼 x,y축과, 마우스포인터에서 카메라에 표시되는 가장 가까운 z좌표를 할당


    }

    private void UpdateBuildImage()
    {
        bool bFocusTile = false;

        if (CurrentFocusTile)
        {
            Tile tile = CurrentFocusTile.GetComponent<Tile>();
            if (!tile.CheckIsOwned())
            {
                Vector3 position = tile.transform.position;
                position.y += BuildDeltaY;
                BuildIconPrefab.transform.position = position;
                bFocusTile = true;

                bool bCanBuild = GameManager.Inst.playerCharacter.CanUseCoin(NormalGuaridanCost);
                Material mat = bCanBuild ? BuildCanMat : BuildCanNotMat;
                BuildIconPrefab.GetComponent<MeshRenderer>().material = mat;
            }
        }

        if (bFocusTile)
        {
            BuildIconPrefab.gameObject.SetActive(true);
        }
        else
        {
            DeActivateBuildImage();
        }
    }

    private void DeActivateBuildImage()
    {
        BuildIconPrefab.gameObject.SetActive(false);
    }

    // TODO : Click Interface? 

    void CheckToBuildGuardian()
    {
        if (CurrentFocusTile != null)
        {
            Tile tile = CurrentFocusTile.GetComponent<Tile>();
            PlayerCharacter player = GameManager.Inst.playerCharacter;
            if (!tile.CheckIsOwned() && player.CanUseCoin(NormalGuaridanCost))
            {
                player.UseCoin(NormalGuaridanCost);

                Vector3 position = BuildIconPrefab.transform.position;
                GameObject guardianInst = Instantiate(GuardianPrefab, position, Quaternion.identity);

                tile.OwnGuardian = guardianInst.GetComponent<Guardian>();

                OnBuild.Invoke();
                DeActivateBuildImage();

                return;
            }

            if (tile && tile.OwnGuardian)
            {
                GameManager.Inst.guardianUpgradeManager.UpgradeGuardian(tile.OwnGuardian);
            }
        }
    }

    private void UpdateKeyInput()
    {
        if (Input.GetMouseButtonUp(0))
        {
            CheckToBuildGuardian();
        }
    }
}