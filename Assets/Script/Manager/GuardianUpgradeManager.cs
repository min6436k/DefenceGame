using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuardianUpgradeManager : MonoBehaviour
{
    public GuardianStatus[] GuardianStatuses; //������� ���� ������ ���� �迭

    public Image AttackRangeImg;
    public Button UpgradeIconButton;

    private Guardian _currentUpgradeGuardian; //���׷��̵��� �����

    public bool bIsUpgrading = false; //���׷��̵� ������
    private bool _isOnButtonHover = false; //��ư�� ���콺�� �ö��ִ���

    public void Start()
    {
        ShowUpgradeIconAndRange(false); //���׷��̵� �̹��� ��Ȱ��ȭ
        GameManager.Inst.guardianBuildManager.OnBuild.AddListener(() => ShowUpgradeIconAndRange(false)); //OnBuild �̺�Ʈ�� ȣ��Ǿ��� �� ���׷��̵� ������ ��Ȱ��ȭ

        UpgradeIconButton.onClick.AddListener(() =>
        {
            Upgrade(_currentUpgradeGuardian); //���׷��̵� ��ư Ŭ�� �� ���� ���õ� ������� ���׷��̵� ��Ű���� ������ �߰�
            Debug.Log("C");
        });
    }

    private void Update()
    {
        UpdateKeyInput();
    }

    public void UpgradeGuardian(Guardian guardian)
    {


        ShowUpgradeIconAndRange(true); //���׷��̵� ������ Ȱ��ȭ
        _currentUpgradeGuardian = guardian;

        Vector3 guardianPos = _currentUpgradeGuardian.transform.position; //������� ������
        Vector3 attackImgPos = Camera.main.WorldToScreenPoint(guardianPos); //UI�� ������� ���� ��ǥ ���� �̵�

        float attackRadius = (_currentUpgradeGuardian.GuardianStatus.AttackRadius) + 1.5f;
        AttackRangeImg.rectTransform.localScale = new Vector3(attackRadius, attackRadius, 1); //���ݹ����� ���� ���� �̹��� ������ ����
        AttackRangeImg.rectTransform.position = attackImgPos;

        UpgradeIconButton.transform.localScale = new Vector3(1 / attackRadius, 1 / attackRadius, 1); //���׷��̵� ��ư�� �θ� ������Ʈ�� �������� �þ ������ ũ��� ����


        bIsUpgrading = true;
    }

    public void ShowUpgradeIconAndRange(bool active)
    {
        AttackRangeImg.gameObject.SetActive(active);
        UpgradeIconButton.gameObject.SetActive(active); //������ ���׷��̵� ��ư �̹��� Ȱ��ȭ
    }

    private void Upgrade(Guardian guardian)
    {
        if (guardian.Level < GuardianStatuses.Length - 1) //�μ��� ���� ������� ������ ������ ����-1(�ε���) ���� �۴ٸ�
        {
            PlayerCharacter player = GameManager.Inst.playerCharacter; //�÷��̾� ��ũ��Ʈ �Ҵ�
            int cost = GuardianStatuses[guardian.Level + 1].UpgradeCost; //���׷��̵� ���� �ҷ�����

            if (player.CanUseCoin(cost)) //���׷��̵� �� ������ �ִٸ�
            {
                player.UseCoin(cost); //���� �Ҹ�
                guardian.Upgrade(GuardianStatuses[guardian.Level + 1]); //���� ����+1�� ���׷��̵� ����
                bIsUpgrading = false; //���׷��̵� �� �� false �Ҵ�
                ShowUpgradeIconAndRange(false); //������ ���׷��̵� ��ư ��Ȱ��ȭ
            }
        }
    }

    public void OnPointerEnter()
    {
        _isOnButtonHover = true;
    }
    public void OnPointerExit()
    {
        _isOnButtonHover = false;
    }

    private void UpdateKeyInput()
    {
        if (Input.GetMouseButtonDown(0)) //���콺 ��Ŭ��
        {
            if (_isOnButtonHover) //��ư ���� ���콺�� �ö� �ִٸ�
            {
                return;
            }

            bIsUpgrading = false; //���׷��̵� �� ������ false �Ҵ�
            ShowUpgradeIconAndRange(false);
        }
    }
}
