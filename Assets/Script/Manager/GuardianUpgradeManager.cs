using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GuardianUpgradeManager : MonoBehaviour
{
    public GuardianStatus[] GuardianStatuses; //가디언의 레벨 정보를 담을 배열

    public Image AttackRangeImg;
    public Button UpgradeIconButton;

    private Guardian _currentUpgradeGuardian; //업그레이드할 가디언

    public bool bIsUpgrading = false; //업그레이드 중인지
    private bool _isOnButtonHover = false; //버튼에 마우스가 올라가있는지

    public void Start()
    {
        ShowUpgradeIconAndRange(false); //업그레이드 이미지 비활성화
        GameManager.Inst.guardianBuildManager.OnBuild.AddListener(() => ShowUpgradeIconAndRange(false)); //OnBuild 이벤트가 호출되었을 시 업그레이드 아이콘 비활성화

        UpgradeIconButton.onClick.AddListener(() =>
        {
            Upgrade(_currentUpgradeGuardian); //업그레이드 버튼 클릭 시 현재 선택된 가디언을 업그레이드 시키도록 리스너 추가
            Debug.Log("C");
        });
    }

    private void Update()
    {
        UpdateKeyInput();
    }

    public void UpgradeGuardian(Guardian guardian)
    {


        ShowUpgradeIconAndRange(true); //업그레이드 아이콘 활성화
        _currentUpgradeGuardian = guardian;

        Vector3 guardianPos = _currentUpgradeGuardian.transform.position; //가디언의 포지션
        Vector3 attackImgPos = Camera.main.WorldToScreenPoint(guardianPos); //UI를 가디언의 월드 좌표 맞춰 이동

        float attackRadius = (_currentUpgradeGuardian.GuardianStatus.AttackRadius) + 1.5f;
        AttackRangeImg.rectTransform.localScale = new Vector3(attackRadius, attackRadius, 1); //공격범위에 맞춰 범위 이미지 스케일 조절
        AttackRangeImg.rectTransform.position = attackImgPos;

        UpgradeIconButton.transform.localScale = new Vector3(1 / attackRadius, 1 / attackRadius, 1); //업그레이드 버튼의 부모 오브젝트의 스케일이 늘어도 일정한 크기로 유지


        bIsUpgrading = true;
    }

    public void ShowUpgradeIconAndRange(bool active)
    {
        AttackRangeImg.gameObject.SetActive(active);
        UpgradeIconButton.gameObject.SetActive(active); //범위와 업그레이드 버튼 이미지 활성화
    }

    private void Upgrade(Guardian guardian)
    {
        if (guardian.Level < GuardianStatuses.Length - 1) //인수로 받은 가디언의 레벨이 레벨의 갯수-1(인덱스) 보다 작다면
        {
            PlayerCharacter player = GameManager.Inst.playerCharacter; //플레이어 스크립트 할당
            int cost = GuardianStatuses[guardian.Level + 1].UpgradeCost; //업그레이드 가격 불러오기

            if (player.CanUseCoin(cost)) //업그레이드 할 코인이 있다면
            {
                player.UseCoin(cost); //코인 소모
                guardian.Upgrade(GuardianStatuses[guardian.Level + 1]); //현재 레벨+1로 업그레이드 실행
                bIsUpgrading = false; //업그레이드 중 에 false 할당
                ShowUpgradeIconAndRange(false); //범위와 업그레이드 버튼 비활성화
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
        if (Input.GetMouseButtonDown(0)) //마우스 좌클릭
        {
            if (_isOnButtonHover) //버튼 위에 마우스가 올라가 있다면
            {
                return;
            }

            bIsUpgrading = false; //업그레이드 중 변수에 false 할당
            ShowUpgradeIconAndRange(false);
        }
    }
}
