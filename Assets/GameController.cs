using System;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public Text playerChoiceText;
    public Text computerChoiceText;
    public Text roundResultText;

    public Button nextButton;
    public Button[] ChoiceButtonList;

    public Slider propabilitySlider;
    public Toggle propabilityTogle;

    private int winCount;
    private int loseCount;


    private void Awake()
    {
        Random.InitState(DateTime.Now.Millisecond);

        nextButton.onClick.AddListener(NextRound);
        for (var index = 0; index < ChoiceButtonList.Length; index++)
        {
            var choiseButton = ChoiceButtonList[index];
            choiseButton.onClick.AddListener(() =>
            {
                OnPlayerChoise(choiseButton);
            });
        }

        propabilitySlider.onValueChanged.AddListener(OnChangePropability);

        ShowRound(true);
    }


    private void OnPlayerChoise(Button choiceBtn)
    {
        var playerIndex = 0;
        while (playerIndex < ChoiceButtonList.Length)
            if (ChoiceButtonList[playerIndex] == choiceBtn) break;
            else playerIndex++;
        playerChoiceText.text = choiceBtn.GetComponentInChildren<Text>().text;

        var computerIndex = Random.Range(0, ChoiceButtonList.Length);

        //ничьи никак не влияют на нечетную игру, поэтому пропускаем их для достоверности
        if (propabilityTogle.isOn && computerIndex != playerIndex) //нечестная игра
        {
            //"нечестный", в этом случае оппонент должен побеждать в соответствии с заданной вероятностью P (число [0..1]) 
            // - т.е. при P=1 оппонент всегда побеждает, при P=0.5 - примерно в половине раундов и т.д.

            var rnd = Random.value;

            if (rnd <= propabilitySlider.value) //должен победить
            {
                computerIndex = playerIndex + 1;
                if (computerIndex >= ChoiceButtonList.Length) computerIndex = 0;
            }
            else //проигрывает или ничья
            {
                computerIndex = playerIndex - 1;
                if (computerIndex < 0) computerIndex = ChoiceButtonList.Length - 1;
            }
        }

        computerChoiceText.text = ChoiceButtonList[computerIndex].GetComponentInChildren<Text>().text;

        ShowResult(playerIndex, computerIndex);

        ShowRound(false);
    }

    private void NextRound()
    {
        ShowRound(true);

        roundResultText.text = string.Format("you {0} vs {1} AI", winCount, loseCount);
    }

    private void ShowRound(bool isStart)
    {
        computerChoiceText.gameObject.SetActive(!isStart);
        playerChoiceText.gameObject.SetActive(!isStart);
        nextButton.gameObject.SetActive(!isStart);
        foreach (var choiceBtn in ChoiceButtonList) choiceBtn.gameObject.SetActive(isStart);
    }

    private void OnChangePropability(float newValue)
    {
        propabilityTogle.GetComponentInChildren<Text>().text = string.Format("{0}% побед", Mathf.RoundToInt(newValue*100));
    }

    private void ShowResult(int playerIndex, int computerIndex)
    {
        if (playerIndex == computerIndex)
        {
            roundResultText.text = "Ничья!";
        }
        else if (playerIndex == computerIndex + 1)
        {
            roundResultText.text = "Победа :)";
            winCount++;
        }
        else if (playerIndex == 0 && computerIndex == ChoiceButtonList.Length - 1)
        {
            roundResultText.text = "Победа ;)";
            winCount++;
        }
        else
        {
            roundResultText.text = "Проиграл :(";
            loseCount++;
        }
    }

}
