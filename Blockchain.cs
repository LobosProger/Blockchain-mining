using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Security.Cryptography;
using System.Text;
public class Blockchain : MonoBehaviour
{
	public string difficulty = "0";
	public string Data;
	public List<Block> BlockchainInfo = new List<Block>();

	[SerializeField]
	private bool Mine;

	private void Update()
	{
		if (Mine)
		{
			Mine = false;
			StartCoroutine(Mining());
		}
	}

	[Serializable]
	public struct Block
	{
		public int block;
		public ulong nonce;
		public string Data;
		public string previousHash;
		public string Hash;

		[Space(40)]
		public double TimeOfMining;

		private string connectedData;
		public string hashingDataWithNonce;

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
			hashingDataWithNonce = connectedData + nonce.ToString();
			Hash = EncryptSHA256(hashingDataWithNonce);
		}

		private string EncryptSHA256(string Data)
		{
			SHA256 sha256 = new SHA256CryptoServiceProvider();
			sha256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Data));
			byte[] result = sha256.Hash;
			StringBuilder strBuilder = new StringBuilder();
			for (int i = 0; i < result.Length; i++)
			{
				strBuilder.Append(result[i].ToString("x2"));
			}
			return strBuilder.ToString();
		}
	}

	IEnumerator Mining()
	{
		Block addingBlock = new Block();
		addingBlock.AssignDataToBlock(Data, BlockchainInfo);

		addingBlock.TimeOfMining = Time.realtimeSinceStartupAsDouble;
		int difficultyAmountOfNone = difficulty.Length;
		addingBlock.MineTheBlock();

		int minedHashes = 0;
		while (addingBlock.Hash.Substring(0, difficultyAmountOfNone) != difficulty)
		{
			addingBlock.MineTheBlock();
			minedHashes++;

			if (minedHashes > 200000)
			{
				Debug.Log("Mining...");
				minedHashes = 0;
				GC.Collect();
				GC.WaitForPendingFinalizers();
			}
		}
		addingBlock.TimeOfMining = Time.realtimeSinceStartupAsDouble - addingBlock.TimeOfMining;
		BlockchainInfo.Add(addingBlock);

		Debug.Log("Block has been mined!\n" + addingBlock.hashingDataWithNonce);
		yield return null;
	}
}