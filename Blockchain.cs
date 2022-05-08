using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using NaughtyAttributes;
[ExecuteAlways]
public class Blockchain : MonoBehaviour
{
	public string difficulty = "0";
	[ResizableTextArea]
	public string Data;
	public List<Block> BlockchainInfo = new List<Block>();

	private double time;
	private bool startMining;

	private void Update()
	{
		if (startMining)
		{
			if (Time.realtimeSinceStartupAsDouble - time >= 2d)
			{
				time = Time.realtimeSinceStartupAsDouble;
				Mine();
			}
		}
	}

	[ButtonMethod]
	private void Mine()
	{
		Block addingBlock = new Block();
		addingBlock.AssignDataToBlock(Data, BlockchainInfo);

		addingBlock.TimeOfMining = Time.realtimeSinceStartupAsDouble;

		int difficultyAmountOfNone = difficulty.Length;

		addingBlock.MineTheBlock();
		while (addingBlock.Hash.Substring(0, difficultyAmountOfNone) != difficulty)
		{
			addingBlock.MineTheBlock();
		}

		addingBlock.TimeOfMining = Time.realtimeSinceStartupAsDouble - addingBlock.TimeOfMining;
		addingBlock.HashrateInSeconds = (addingBlock.nonce / addingBlock.TimeOfMining) * 1000;
		BlockchainInfo.Add(addingBlock);

		Debug.Log(addingBlock.ReturnHashData());
	}

	[System.Serializable]
	public struct Block
	{
		public int block;
		public ulong nonce;
		[ResizableTextArea]
		public string Data;
		[ResizableTextArea]
		public string previousHash;
		[ResizableTextArea]
		public string Hash;

		[Space(40)]
		public double TimeOfMining;
		public double HashrateInSeconds;


		private string connectedData;
		private string hashingData;

		public void AssignDataToBlock(string data, List<Block> blockchain)
		{
			nonce = 0;
			Hash = "a";
			block = blockchain.Count + 1;
			Data = data;
			previousHash = blockchain.Count > 0 ? blockchain[block - 2].Hash : "0";

			connectedData = block.ToString() + data + previousHash;
		}

		public void MineTheBlock()
		{
			nonce++;
			hashingData = connectedData + nonce.ToString();
			Hash = LobosRoboticsFunctions.EncryptSHA256(ReturnHashData());
		}

		public string ReturnHashData()
		{
			return hashingData;
		}
	}
	[ButtonMethod]
	private void StartMining() => startMining = true;
	[ButtonMethod]
	private void StopMining() => startMining = false;
}
