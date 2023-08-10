using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FireBase.Models;
using FirebaseWebGL.Scripts.FirebaseBridge;
using Newtonsoft.Json;
using UI;
using UnityEngine;

namespace FireBase
{
	public class FirebaseTest : MonoBehaviour
	{

		public static FirebaseTest Instance;
		private string _playerName = "f0kes";
		private LeaderboardModel _leaderboard;
		private TaskCompletionSource<LeaderboardModel> _leaderboardTask;
		private void Awake()
		{
			if(Instance == null)
				Instance = this;
			else
				Destroy(this);
			transform.SetParent(null);
			DontDestroyOnLoad(gameObject);
		}
		private void OnDestroy()
		{
			if(Instance == this)
				Instance = null;
		}
		private void Start()
		{
			if(Application.platform != RuntimePlatform.WebGLPlayer)
			{
				TextMessageRenderer.Instance.ShowMessage("The code is not running on a WebGL build; as such, the Javascript functions will not be recognized.");
				Destroy(gameObject);
			}
		}
		public void SetPlayerName(string playerName)
		{
			_playerName = playerName;
		}

		#region Requests

		public void PushGameEntry(GameEntryModel model)
		{
			var path = "/Players/" + model.PlayerName + "/GameEntries";
			var json = JsonConvert.SerializeObject(model);
			Debug.Log("model: " + json);
			FirebaseDatabase.PushJSON(path, json, gameObject.name, "DoNothing", "DisplayErrorObject");
		}
		public async Task<LeaderboardModel> RequestLeaderboard()
		{
			var path = "/Players";
			FirebaseDatabase.GetJSON(path, gameObject.name, "GetLeaderboardCallback", "DisplayErrorObject");
			Debug.Log("waiting for leaderboard");
			_leaderboardTask = new TaskCompletionSource<LeaderboardModel>();
			Debug.Log("waiting for leaderboard 2");
			return await _leaderboardTask.Task;
		}

		#endregion

		#region Callbacks

		public void DoNothing(string message)
		{
			Debug.Log(message);
		}
		public void DisplayErrorObject(string error)
		{
			TextMessageRenderer.Instance.ShowMessage(error);
			Debug.LogError(error);
		}
		public void GetLeaderboardFallback(string json)
		{
			TextMessageRenderer.Instance.ShowMessage(json);
			_leaderboardTask.SetResult(null);
		}
		public void GetLeaderboardCallback(string json)
		{
			Debug.Log("json: " + json);
			var players = JsonConvert.DeserializeObject<Dictionary<string, PlayerModel>>(json);
			if(players == null)
				return;
			Debug.Log("players: " + players.Count);
			_leaderboard = new LeaderboardModel();
			Debug.Log("leaderboard: " + _leaderboard);
			foreach(var (playerName, value) in players)
			{
				var gameEntries = value.GameEntries;
				if(gameEntries == null)
					continue;
				foreach(var (id, model) in gameEntries)
				{
					_leaderboard.AddEntry(playerName, model);
				}
			}
			Debug.Log("leaderboard 2: " + _leaderboard);
			_leaderboard.Sort();
			Debug.Log("leaderboard 3: " + _leaderboard);
			_leaderboardTask.SetResult(_leaderboard);
			Debug.Log("leaderboard 4: " + _leaderboard);
		}

		#endregion

	}
}