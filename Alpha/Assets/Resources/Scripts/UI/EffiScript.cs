using UnityEngine;
using System.Collections;
using PlaneMan = Singleton<PlaneManager>;

public class EffiScript : MonoBehaviour
{
    [Header("Efficency Score Checks")]
    public float FirstStarCheck;
    public float SecondStarCheck;
    public float ThirdStarCheck;
    public float FourthStarCheck;
    public float FifthStarCheck;

    [Header("Efficency")]
    public Transform StarOne;
    public Transform StarTwo;
    public Transform StarThree;
    public Transform StarFour;
    public Transform StarFive;

    [Header("Game Over Efficency")]
    public Transform EndStarOne;
    public Transform EndStarTwo;
    public Transform EndStarThree;
    public Transform EndStarFour;
    public Transform EndStarFive;

    ObservedValue<float> CurrentScore;

    void Start ()
    {
        CurrentScore = new ObservedValue<float>(PlaneMan.Instance.m_Score, UpdateStarCount);
	}

	void Update ()
    {
        CurrentScore.Value = PlaneMan.Instance.m_Score;
    }

    private void UpdateStarCount(float oldVal, float newVal)
    {
        if (oldVal != newVal)
        {
            if (newVal >= FirstStarCheck)
            {
                StarOne.gameObject.SetActive(true);
                EndStarOne.gameObject.SetActive(true);
            }
            if (newVal >= SecondStarCheck)
            {
                StarTwo.gameObject.SetActive(true);
                EndStarTwo.gameObject.SetActive(true);
            }
            if (newVal >= ThirdStarCheck && PlaneMan.Instance.m_CrashedAircraft < 4)
            {
                StarThree.gameObject.SetActive(true);
                EndStarThree.gameObject.SetActive(true);
            }
            if (newVal >= FourthStarCheck && PlaneMan.Instance.m_CrashedAircraft < 3)
            {
                StarFour.gameObject.SetActive(true);
                EndStarFour.gameObject.SetActive(true);
            }
            if (newVal >= FifthStarCheck && PlaneMan.Instance.m_CrashedAircraft < 2)
            {
                StarFive.gameObject.SetActive(true);
                EndStarFive.gameObject.SetActive(true);
            }

            if (PlaneMan.Instance.m_CrashedAircraft == 2)
            {
                StarFive.gameObject.SetActive(false);
                EndStarFive.gameObject.SetActive(false);
            }
            if (PlaneMan.Instance.m_CrashedAircraft == 3)
            {
                StarFour.gameObject.SetActive(false);
                EndStarFour.gameObject.SetActive(false);

                StarFive.gameObject.SetActive(false);
                EndStarFive.gameObject.SetActive(false);
            }
            if (PlaneMan.Instance.m_CrashedAircraft == 4 
                || PlaneMan.Instance.m_CrashedAircraft == 5)
            {
                StarThree.gameObject.SetActive(false);
                EndStarThree.gameObject.SetActive(false);

                StarFour.gameObject.SetActive(false);
                EndStarFour.gameObject.SetActive(false);

                StarFive.gameObject.SetActive(false);
                EndStarFive.gameObject.SetActive(false);
            }

            if (PlaneMan.Instance.m_CrashedAircraft == 6)
            {
                StarTwo.gameObject.SetActive(false);
                EndStarTwo.gameObject.SetActive(false);

                StarThree.gameObject.SetActive(false);
                EndStarThree.gameObject.SetActive(false);

                StarFour.gameObject.SetActive(false);
                EndStarFour.gameObject.SetActive(false);

                StarFive.gameObject.SetActive(false);
                EndStarFive.gameObject.SetActive(false);
            }

            if (PlaneMan.Instance.m_CrashedAircraft >= 7)
            {
                StarOne.gameObject.SetActive(false);
                EndStarOne.gameObject.SetActive(false);

                StarTwo.gameObject.SetActive(false);
                EndStarTwo.gameObject.SetActive(false);

                StarThree.gameObject.SetActive(false);
                EndStarThree.gameObject.SetActive(false);

                StarFour.gameObject.SetActive(false);
                EndStarFour.gameObject.SetActive(false);

                StarFive.gameObject.SetActive(false);
                EndStarFive.gameObject.SetActive(false);
            }
        }
    }
}
