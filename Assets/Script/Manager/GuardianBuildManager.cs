using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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


        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,100, LayerMask.GetMask("Tile")))
        {
            if (hit.collider.CompareTag("Tile")) CurrentFocusTile = hit.collider.gameObject;
        }


        //ScreenToWorldPoint 방식

        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));

        //if (Physics.Raycast(Camera.main.transform.position, (mousePosition - Camera.main.transform.position).normalized, out RaycastHit hit, 500, LayerMask.GetMask("Tile")))
        //{
        //    if (hit.collider.CompareTag("Tile")) CurrentFocusTile = hit.collider.gameObject;
        //}
    }

    private void UpdateBuildImage()
    {
        bool bFocusTile = false; //현재 포커스된 타일 유무를 false로 초기홤

        if (CurrentFocusTile) //현재 포커스된 타일이 있다면
        {
            Tile tile = CurrentFocusTile.GetComponent<Tile>(); //해당 타일 오브젝트의 타일 스크립트 할당
            if (!tile.CheckIsOwned()) //해당 타일에 가디언이 없을 시
            {
                Vector3 position = tile.transform.position;
                position.y += BuildDeltaY; //해당 타일의 위치에 BuildDeltaY를 더해줌
                BuildIconPrefab.transform.position = position; //빌드아이콘의 위치를 구한 position으로 변경
                bFocusTile = true; //현재 포커스된 타일 유무를 true로 변경

                bool bCanBuild = GameManager.Inst.playerCharacter.CanUseCoin(NormalGuaridanCost); //가디언을 제작할 만큼의 코인이 있는지 검사
                Material mat = bCanBuild ? BuildCanMat : BuildCanNotMat; //마테리얼 변수에 bCanBuild가 true라면 BuildCanMat이미지,false라면 BuildCanNotMat 이미지 할당
                BuildIconPrefab.GetComponent<MeshRenderer>().material = mat; //빌드아이콘 오브젝트에 마테리얼 적용
            }
        }

        if (bFocusTile) //현재 포커스된 타일이 있다면
        {
            BuildIconPrefab.gameObject.SetActive(true); //빌드아이콘 활성화
        }
        else
        {
            DeActivateBuildImage(); 
        }
    }

    private void DeActivateBuildImage()
    {
        BuildIconPrefab.gameObject.SetActive(false);//빌드아이콘 비활성화
    }

    // TODO : Click Interface? 

    void CheckToBuildGuardian()
    {
        if (CurrentFocusTile != null) //포커스된 타일이 null이 아니라면
        {
            Tile tile = CurrentFocusTile.GetComponent<Tile>();//해당 타일 오브젝트의 타일 스크립트 할당
            PlayerCharacter player = GameManager.Inst.playerCharacter; //플레이어 캐릭터 스크립트 할당
            if (!tile.CheckIsOwned() && player.CanUseCoin(NormalGuaridanCost)) //해당 타일에 가디언이 없고 제작할 코인도 있을 경우
            {
                player.UseCoin(NormalGuaridanCost); //코인 감소

                Vector3 position = BuildIconPrefab.transform.position; //가디언을 생성할 포지션
                GameObject guardianInst = Instantiate(GuardianPrefab, position, Quaternion.identity); //가디언 프리팹 생성

                tile.OwnGuardian = guardianInst.GetComponent<Guardian>(); //타일의 변수에 생성한 가디언의 스크립트 할당

                OnBuild.Invoke(); //이벤트 호출
                DeActivateBuildImage();

                return;
            }

            if (tile && tile.OwnGuardian) //해당 타일에 가디언이 이미 있을 경우
            {
                GameManager.Inst.guardianUpgradeManager.UpgradeGuardian(tile.OwnGuardian); //UpgradeGuardian함수 호출
            }
        }
    }

    private void UpdateKeyInput()
    {
        if (Input.GetMouseButtonUp(0)) //마우스 클릭 감지
        {
            CheckToBuildGuardian();
        }
    }
}