//-----------------------------------------------------------------------------------------------------------------
// 
//	Mushroom Example
//	Created by : Luis Filipe (filipe@seines.pt)
//	Dec 2010
//
//	Source code in this example is in the public domain.
//  The naruto character model in this demo is copyrighted by Ben Mathis.
//  See Assets/Models/naruto.txt for more details
//
//-----------------------------------------------------------------------------------------------------------------

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PlayerIOClient;
using UnityEngine.SceneManagement;

public class ChatEntry {
	public string text = "";
	public bool mine = true;
}

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public Connection pioconnection;
	private List<Message> msgList = new List<Message>(); //  Messsage queue implementation
	public bool joinedroom = false;

	public GameObject ActivePlayer;
	public GameObject EmptyPlayerPrefab;

	public List<GameObject> otherPlayers;


	// UI stuff
	private string infomsg = "";
	private void Awake()
	{
		instance = this;
	}

	void Start() {
		Application.runInBackground = true;

		// Create a random userid 
		System.Random random = new System.Random();
		string userid = "Guest" + random.Next(0, 10000);

		Debug.Log("Starting");

		PlayerIO.Authenticate(
			"sunny-garden-server-ixvbjpjcseomtptcow2lsq",            //Your game id
			"public",                               //Your connection id
			new Dictionary<string, string> {        //Authentication arguments
				{ "userId", userid },
			},
			null,                                   //PlayerInsight segments
			delegate (Client client) {
				Debug.Log("Successfully connected to Player.IO");
				infomsg = "Successfully connected to Player.IO";

				Debug.Log("Create ServerEndpoint");
				// Comment out the line below to use the live servers instead of your development server
				client.Multiplayer.DevelopmentServer = new ServerEndpoint("localhost", 8184);

				Debug.Log("CreateJoinRoom");
				//Create or join the room 
				client.Multiplayer.CreateJoinRoom(
					"UnityDemoRoom",                    //Room id. If set to null a random roomid is used
					"UnitySunnyGarden",                   //The room type started on the server
					true,                               //Should the room be visible in the lobby?
					null,
					null,
					delegate (Connection connection) {
						Debug.Log("Joined Room.");
						infomsg = "Joined Room.";
						// We successfully joined a room so set up the message handler
						pioconnection = connection;
						pioconnection.OnMessage += handlemessage;
						joinedroom = true;


					},
					delegate (PlayerIOError error) {
						Debug.Log("Error Joining Room: " + error.ToString());
						infomsg = error.ToString();
					}
				);
			},
			delegate (PlayerIOError error) {
				Debug.Log("Error connecting: " + error.ToString());
				infomsg = error.ToString();
			}
		);

	}

	void handlemessage(object sender, Message m) {
		msgList.Add(m);
	}

	void FixedUpdate() {
		// process message queue
		foreach (Message m in msgList) {
			GameObject player = null;
			string id;
			switch (m.Type) {

				case "PlayerJoined":
					//print("joined");
					GameObject go = Instantiate(EmptyPlayerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
					otherPlayers.Add(go);
					go.GetComponent<PlayerShell>().id = (m.GetString(0));
					pioconnection.Send("CoordAsk", m.GetString(0));
					break;

				case "CoordAsk":
					pioconnection.Send("CoordAnswer", m.GetString(0), PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y, PlayerController.instance.HP);
					break;

				case "CoordAnswer":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}
					player.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
					player.GetComponent<PlayerShell>().HP = m.GetInt(3);
					break;

				case "MapRequest":
					print("received map request");
					for (int i = 0; i<SkellyManager.instance.Skeletons.Count; i++)
                    {
						SkeletonControl sk = SkellyManager.instance.Skeletons[i];
						sk.SendPosition(i, m.GetString(0));
                    }

					foreach (Tree t in TreesManager.instance.UsableTrees)
                    {
						t.SendState(m.GetString(0));
                    }

					foreach (CarrotGrowth c in CarrotManager.instance.UsableCarrots)
                    {
						c.SendState(m.GetString(0));
                    }
					break;

				case "Skelly":
					print("recieved skelly");
					if (m.GetInt(0) < SkellyManager.instance.Skeletons.Count)
					{
						SkeletonControl sk = SkellyManager.instance.Skeletons[m.GetInt(0)];
						Vector3 position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
						sk.transform.position = position;
						sk.GetComponentInChildren<Skelly_AnimationInterface>().hitPoints = m.GetInt(3);
					}
					else
					{
						if (m.GetBoolean(4))
                        {
							GameObject skel = Instantiate(SkellyManager.instance.Skeletons[0].gameObject, new Vector3(m.GetFloat(1), m.GetFloat(2), 0), Quaternion.identity);
							skel.GetComponentInChildren<Skelly_AnimationInterface>().hitPoints = m.GetInt(3);
						}
                        else
                        {
							GameObject skel = Instantiate(PlayerController.instance.skellySoul, new Vector3(m.GetFloat(1), m.GetFloat(2), 0), Quaternion.identity);
							skel.GetComponentInChildren<Skelly_AnimationInterface>().hitPoints = m.GetInt(3);
						}

					}
					break;

				case "TreeInfo":
					print("recieved tree");
					TreesManager.instance.TreeFinder(m.GetInt(0), m.GetInt(1)).AdjustAtSpawn(m.GetInt(2), m.GetBoolean(3), m.GetFloat(4));
					break;

				case "CarrotInfo":
					print("recieved carrot");
					CarrotManager.instance.CarrotFinder(m.GetInt(0), m.GetInt(1)).AdjustAtSpawn(m.GetInt(2), m.GetFloat(3));
					break;

				case "Sprinting":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
                    {
						if (pl.GetComponent<PlayerShell>().id == id)
                        {
							player = pl;
							break;
                        }
                    }
					player.GetComponent<AnimationInterface>().isSprinting = m.GetBoolean(1);
					break;

				case "FacingRight":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}

					player.GetComponent<PlayerShell>().facingRight = m.GetBoolean(1);
					player.GetComponent<PlayerShell>().ChangeOrientation((m.GetBoolean(1))? 0 : 180);
					break;
				case "Position":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}

					player.transform.position = new Vector3(m.GetFloat(1), m.GetFloat(2), 0);
					break;

				case "PlayerLeft":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}
					otherPlayers.Remove(player);
					Destroy(player.gameObject);

					break;

				case "Action":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}
					player.GetComponent<PlayerShell>().Action(m.GetBoolean(1), m.GetInt(2));

					break;

				case "Moving":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}
					player.GetComponent<AnimationInterface>().isMoving = m.GetBoolean(1);
					break;

				case "Damaged":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}
					player.GetComponent<AnimationInterface>().gotHit = true;
					player.GetComponent<PlayerShell>().HP = m.GetInt(1);

					Instantiate(PlayerController.instance.DamageParticles, player.transform.position + Vector3.up * 0.25f - Vector3.forward, Quaternion.identity);
					break;

				case "Died":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}
					player.GetComponent<AnimationInterface>().isDead = true;
					Instantiate(PlayerController.instance.skellySoul, player.transform.position, Quaternion.identity);
					break;

				case "RespawnPlayer":
					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}
					player.GetComponent<AnimationInterface>().respawn = true;
					player.gameObject.transform.position = Vector3.zero;
					break;

				case "ActionHit":
					//print("Action : " +m.GetString(0) + " has used a " + ((Tool)m.GetInt(3)).ToString() +" at " + m.GetInt(1) + " " + m.GetInt(2) );

					id = (m.GetString(0));
					foreach (GameObject pl in otherPlayers)
					{
						if (pl.GetComponent<PlayerShell>().id == id)
						{
							player = pl;
							break;
						}
					}
					if (player == null)
					{ player = PlayerController.instance.gameObject; }


					if (m.GetInt(3) == (int)Tool.Axe)
                    {
						TreesManager.instance.TreeHit(m.GetInt(1),m.GetInt(2), player);
                    }
					else if (m.GetInt(3) == (int)Tool.Shovel)
					{
						CarrotManager.instance.CarrotDig(m.GetInt(1), m.GetInt(2), player);
					}
					else if (m.GetInt(3) == (int)Tool.Sword)
                    {
						SkellyManager.instance.SwordSwing(m.GetInt(1), m.GetInt(2));
                    }
					break;



			}
		}

		// clear message queue after it's been processed
		msgList.Clear();
	}

	public void QuitGame()
    {
		Application.Quit();
    }

	public void LoadGameScene()
    {
		SceneManager.LoadScene(1);
    }

	void OnGUI() {
		if (infomsg != "") {
			GUI.Label(new Rect(10, 180, Screen.width, 20), infomsg);
		}
	}

	
}
