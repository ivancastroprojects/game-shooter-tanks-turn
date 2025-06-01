using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class TurnPlayer : MonoBehaviour
{
    public GameObject player1;
    public GameObject player2;
    public GameObject computer;
    public GameObject textTurn;

    private Vector3[] computerPos = new[] { new Vector3(-10.7f, -0.83f, 130.1f),
        new Vector3(-105f, -0.83f, -24.5f), new Vector3(-11.9f, -0.83f, -118.9f),
        new Vector3(115.5f, -0.83f, -89.2f), new Vector3(97f, -0.83f, 66.4f),
        new Vector3(-12.4f, -0.83f, 25.3f)
    };

    //var mode game
    public bool modePlayers;
    //var who is the next in turn
    public bool player1_actived;

    private void Awake()
    {
        int gameMode = PlayerPrefs.GetInt("gameMode");
        int selected = Random.Range(0, 5);

        //Spawning in random position the tanks in the map
        var position1 = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));
        var position2 = new Vector3(Random.Range(-100, 100), 0, Random.Range(-100, 100));

        player1_actived = true;
        player1.transform.position = position1;

        GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).GetComponent<Text>().text = "Turn of Player 1";
        gameObject.GetComponent<AudioSource>().Play();

        //If gameMode == 1 will be Player VS Player
        if (gameMode == 1)
        {
            modePlayers = true;
            computer.gameObject.SetActive(false);
            player2.transform.position = position2;

            player1.transform.GetChild(3).gameObject.SetActive(false);
            player2.transform.GetChild(3).gameObject.SetActive(true);

            player2.GetComponent<ThirdPersonInput>().enabled = false;
            player2.GetComponent<TankController>().enabled = false;
            player2.GetComponent<TankShooting>().enabled = false;
            player2.GetComponent<AudioSource>().enabled = false;
            player2.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);

        }
        //If is other, will be Player VS Computer
        else
        {
            modePlayers = false;
            player2.gameObject.SetActive(false);
            player1.transform.GetChild(3).gameObject.SetActive(false);
            computer.transform.GetChild(3).gameObject.SetActive(true);
            computer.transform.position = computerPos[selected];

            computer.GetComponent<NavMeshAgent>().enabled = false;
            computer.GetComponent<EnemyAIComputer>().enabled = false;
            computer.GetComponent<AudioSource>().enabled = false;
            computer.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);
        }

        StartCoroutine(WaitingFor());
    }
    public void isYourTurn()
    {
        //Corroutine that wait you 3 seconds to repositionate your tank before next turn
        StartCoroutine(WaitingFor());

        gameObject.GetComponent<AudioSource>().Play();

        //Who is the owner of next turn?
        //If Player1 is actived, we disable his components and active Player 2, same to Computer mode

        //PLAYER VS PLAYER
        if (modePlayers)
        {
            Debug.Log("Entrando mode PLAYER");
            computer.gameObject.SetActive(false);
            if (player1_actived)
            {
                GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).GetComponent<Text>().text = "Turn of Player 2";

                player1.GetComponent<ThirdPersonInput>().enabled = false;
                player1.GetComponent<TankController>().enabled = false;
                player1.GetComponent<TankShooting>().enabled = false;
                player1.GetComponent<AudioSource>().enabled = false;

                player1.transform.GetChild(3).gameObject.SetActive(true);
                player1.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);

                player2.GetComponent<ThirdPersonInput>().enabled = true;
                player2.GetComponent<TankController>().enabled = true;
                player2.GetComponent<TankController>().Start();
                player2.GetComponent<TankShooting>().enabled = true;
                player2.GetComponent<AudioSource>().enabled = true;

                player2.transform.GetChild(3).gameObject.SetActive(false);
                player2.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
                player1_actived = false;
            }
            else
            {
                GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).GetComponent<Text>().text = "Turn of Player 1";

                player2.GetComponent<ThirdPersonInput>().enabled = false;
                player2.GetComponent<TankController>().enabled = false;
                player2.GetComponent<TankShooting>().enabled = false;
                player2.GetComponent<AudioSource>().enabled = false;

                player2.transform.GetChild(3).gameObject.SetActive(true);
                player2.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);

                computer.GetComponent<EnemyAIComputer>().enabled = false;
                computer.GetComponent<NavMeshAgent>().enabled = false;
                computer.GetComponent<AudioSource>().enabled = false;

                player1.GetComponent<ThirdPersonInput>().enabled = true;
                player1.GetComponent<TankController>().enabled = true;
                player1.GetComponent<TankController>().Start();
                player1.GetComponent<TankShooting>().enabled = true;
                player1.GetComponent<AudioSource>().enabled = true;

                player1.transform.GetChild(3).gameObject.SetActive(false);
                player1.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
                player1_actived = true;
            }
        }
        //PLAYER VS COMPUTER
        else
        {
            Debug.Log("Entrando mode COMPUTER");

            player2.SetActive(false);
            if (player1_actived)
            {
                GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).GetComponent<Text>().text = "Turn of Computer";

                player1.GetComponent<ThirdPersonInput>().enabled = false;
                player1.GetComponent<TankController>().enabled = false;
                player1.GetComponent<TankShooting>().enabled = false;
                player1.GetComponent<AudioSource>().enabled = false;

                player1.transform.GetChild(3).gameObject.SetActive(true);
                player1.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);

                computer.GetComponent<EnemyAIComputer>().enabled = true;
                computer.GetComponent<EnemyAIComputer>().Start();
                computer.GetComponent<NavMeshAgent>().enabled = true;
                computer.GetComponent<AudioSource>().enabled = true;

                computer.transform.GetChild(3).gameObject.SetActive(false);
                computer.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
                player1_actived = false;
            }
            else
            {
                GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).gameObject.SetActive(true);
                GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).GetComponent<Text>().text = "Turn of Player 1";

                computer.GetComponent<EnemyAIComputer>().enabled = false;
                computer.GetComponent<NavMeshAgent>().enabled = false;
                computer.GetComponent<AudioSource>().enabled = false;

                computer.transform.GetChild(3).gameObject.SetActive(true);
                computer.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(false);

                player1.GetComponent<ThirdPersonInput>().enabled = true;
                player1.GetComponent<TankController>().enabled = true;
                player1.GetComponent<TankController>().Start();
                player1.GetComponent<TankShooting>().enabled = true;
                player1.GetComponent<AudioSource>().enabled = true;

                player1.transform.GetChild(3).gameObject.SetActive(false);
                player1.transform.GetChild(2).transform.GetChild(1).gameObject.SetActive(true);
                player1_actived = true;
            }
        }
    }

    public IEnumerator WaitingFor()
    {
        yield return new WaitForSeconds(2);
        GameObject.FindGameObjectWithTag("Canvas").transform.GetChild(6).gameObject.SetActive(false);
    }

    public void LoadNewLevel(int indexScene)
    {
        SceneManager.LoadSceneAsync(indexScene);
    }

    public void WinnerGame()
    {
        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().enabled = true;
        gameObject.GetComponent<AudioSource>().clip = (AudioClip)Resources.Load("Assets/Sounds/victory");
        gameObject.GetComponent<AudioSource>().Play();

        gameObject.transform.GetChild(8).GetComponent<Text>().enabled = true;

        StartCoroutine(WaitingForEnd());
    }

    //Going to main menu
    public IEnumerator WaitingForEnd()
    {
        yield return new WaitForSeconds(3);
        LoadNewLevel(0);
    }
}
