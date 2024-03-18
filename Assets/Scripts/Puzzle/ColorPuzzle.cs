using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPuzzle : MonoBehaviour
{
    public string correctPassword;      // 올바른 암호
    private string enteredPassword = ""; // 사용자가 입력한 암호

    public Text passwordText;     // 암호를 표시할 텍스트 UI
    public Image correctImage;    // 정답 시 변경될 이미지

    private Color originalColor;  // 원래 이미지 색상
    public Color wrongColor;      // 잘못된 암호일 때 변경될 색상
    public float wrongColorDuration = 1.0f;  // 잘못된 암호 색상 유지 시간

    private bool isCheckingPassword = false;  // 암호 체크 중인지 여부

    private void Start()
    {
        UpdatePasswordText(); // 초기화 시 텍스트 업데이트
        originalColor = correctImage.color;  // 원래 이미지 색상 저장
    }

    public void ButtonPressed(string buttonValue)
    {
        if (isCheckingPassword)
        {
            return;  // 암호 체크 중인 경우 입력 무시
        }

        enteredPassword += buttonValue;  // 입력한 암호에 숫자 추가
        UpdatePasswordText();             // 텍스트 업데이트

        // 암호 길이가 올바른 암호의 길이와 같을 때 체크
        if (enteredPassword.Length == correctPassword.Length)
        {
            CheckPassword();
        }
    }

    private void CheckPassword()
    {
        isCheckingPassword = true;  // 암호 체크 중임을 표시

       
        if (enteredPassword == correctPassword)
        {
            OpenDoor();
        }
        else
        {
            StartCoroutine(ShowWrongColor());
        }
    }

    private void OpenDoor()
    {
        correctImage.color = Color.green;  // 이미지 색상 변경
    }

    private void ResetPassword()
    {
        enteredPassword = "";  // 입력한 암호 초기화
        UpdatePasswordText();  // 텍스트 업데이트
    }

    private void UpdatePasswordText()
    {
        passwordText.text = enteredPassword;  // 텍스트 업데이트
    }

    private System.Collections.IEnumerator ShowWrongColor()
    {
        correctImage.color = wrongColor;  // 잘못된 암호 색상으로 변경

        yield return new WaitForSeconds(wrongColorDuration);  // 지정한 시간 동안 대기

        correctImage.color = originalColor;  // 원래 색상으로 되돌림
        ResetPassword();  // 암호 초기화

        isCheckingPassword = false;  // 암호 체크 상태 해제
    }
}
