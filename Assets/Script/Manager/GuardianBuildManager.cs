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

    public GameObject CurrentFocusTile; //���� ���õ� Ÿ��
    public GameObject GuardianPrefab;
    public GameObject BuildIconPrefab;

    public Material BuildCanMat; //������ �� ���� ���� ���׸���
    public Material BuildCanNotMat; //������ �� ���� ���� ���׸���

    public float BuildDeltaY = 0f; //���� �� ������� y��ǥ�� ���� ��
    public float FocusTileDistance = 0.05f; //��Ŀ���� �ν��� �Ÿ�

    public int NormalGuaridanCost = 50; //������� ����

    public UnityEvent OnBuild;


    void Start()
    {
        Tiles = GameObject.FindGameObjectsWithTag("Tile"); //���� �ִ� Tile �±׸� ���� ������Ʈ���� �迭�� �Ҵ�
        BuildIconPrefab = Instantiate(BuildIconPrefab, transform.position, Quaternion.Euler(90, 0, 0)); //���� ������ �������� ����
        BuildIconPrefab.gameObject.SetActive(false); //������ �������� ��Ȱ��ȭ
    }

    void Update()
    {
        bool bisUpgrading = GameManager.Inst.guardianUpgradeManager.bIsUpgrading; //guardianUpgradeManager���� ���׷��尡 ���������� ���� �Ҵ�

        UpdateFindFocusTile();
        if (!bisUpgrading)
        {
            UpdateBuildImage();
            UpdateKeyInput();
        }
    }

    private void UpdateFindFocusTile()
    {
        CurrentFocusTile = null; //���� ��Ŀ������ Ÿ�� null�� �ʱ�ȭ


        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit,100, LayerMask.GetMask("Tile")))
        {
            if (hit.collider.CompareTag("Tile")) CurrentFocusTile = hit.collider.gameObject;
        }


        //ScreenToWorldPoint ���

        //Vector3 mousePosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.farClipPlane));

        //if (Physics.Raycast(Camera.main.transform.position, (mousePosition - Camera.main.transform.position).normalized, out RaycastHit hit, 500, LayerMask.GetMask("Tile")))
        //{
        //    if (hit.collider.CompareTag("Tile")) CurrentFocusTile = hit.collider.gameObject;
        //}
    }

    private void UpdateBuildImage()
    {
        bool bFocusTile = false; //���� ��Ŀ���� Ÿ�� ������ false�� �ʱ��c

        if (CurrentFocusTile) //���� ��Ŀ���� Ÿ���� �ִٸ�
        {
            Tile tile = CurrentFocusTile.GetComponent<Tile>(); //�ش� Ÿ�� ������Ʈ�� Ÿ�� ��ũ��Ʈ �Ҵ�
            if (!tile.CheckIsOwned()) //�ش� Ÿ�Ͽ� ������� ���� ��
            {
                Vector3 position = tile.transform.position;
                position.y += BuildDeltaY; //�ش� Ÿ���� ��ġ�� BuildDeltaY�� ������
                BuildIconPrefab.transform.position = position; //����������� ��ġ�� ���� position���� ����
                bFocusTile = true; //���� ��Ŀ���� Ÿ�� ������ true�� ����

                bool bCanBuild = GameManager.Inst.playerCharacter.CanUseCoin(NormalGuaridanCost); //������� ������ ��ŭ�� ������ �ִ��� �˻�
                Material mat = bCanBuild ? BuildCanMat : BuildCanNotMat; //���׸��� ������ bCanBuild�� true��� BuildCanMat�̹���,false��� BuildCanNotMat �̹��� �Ҵ�
                BuildIconPrefab.GetComponent<MeshRenderer>().material = mat; //��������� ������Ʈ�� ���׸��� ����
            }
        }

        if (bFocusTile) //���� ��Ŀ���� Ÿ���� �ִٸ�
        {
            BuildIconPrefab.gameObject.SetActive(true); //��������� Ȱ��ȭ
        }
        else
        {
            DeActivateBuildImage(); 
        }
    }

    private void DeActivateBuildImage()
    {
        BuildIconPrefab.gameObject.SetActive(false);//��������� ��Ȱ��ȭ
    }

    // TODO : Click Interface? 

    void CheckToBuildGuardian()
    {
        if (CurrentFocusTile != null) //��Ŀ���� Ÿ���� null�� �ƴ϶��
        {
            Tile tile = CurrentFocusTile.GetComponent<Tile>();//�ش� Ÿ�� ������Ʈ�� Ÿ�� ��ũ��Ʈ �Ҵ�
            PlayerCharacter player = GameManager.Inst.playerCharacter; //�÷��̾� ĳ���� ��ũ��Ʈ �Ҵ�
            if (!tile.CheckIsOwned() && player.CanUseCoin(NormalGuaridanCost)) //�ش� Ÿ�Ͽ� ������� ���� ������ ���ε� ���� ���
            {
                player.UseCoin(NormalGuaridanCost); //���� ����

                Vector3 position = BuildIconPrefab.transform.position; //������� ������ ������
                GameObject guardianInst = Instantiate(GuardianPrefab, position, Quaternion.identity); //����� ������ ����

                tile.OwnGuardian = guardianInst.GetComponent<Guardian>(); //Ÿ���� ������ ������ ������� ��ũ��Ʈ �Ҵ�

                OnBuild.Invoke(); //�̺�Ʈ ȣ��
                DeActivateBuildImage();

                return;
            }

            if (tile && tile.OwnGuardian) //�ش� Ÿ�Ͽ� ������� �̹� ���� ���
            {
                GameManager.Inst.guardianUpgradeManager.UpgradeGuardian(tile.OwnGuardian); //UpgradeGuardian�Լ� ȣ��
            }
        }
    }

    private void UpdateKeyInput()
    {
        if (Input.GetMouseButtonUp(0)) //���콺 Ŭ�� ����
        {
            CheckToBuildGuardian();
        }
    }
}