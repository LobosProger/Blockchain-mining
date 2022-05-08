using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using MyBox;
using NaughtyAttributes;
using System.Security.Cryptography;
using System.IO;
using System.Text;
[ExecuteAlways]
public class Blockchain : MonoBehaviour
{
	public string difficulty = "0";
	[ResizableTextArea]
	public string Data;
	public List<Block> BlockchainInfo = new List<Block>();

	[ButtonMethod]
	private void Mine()
	{
		StartCoroutine(Mining());
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

		ulong minedHashes = 0;
		long totalBytesCleared = 0;
		while (addingBlock.Hash.Substring(0, difficultyAmountOfNone) != difficulty)
		{
			addingBlock.MineTheBlock();
			minedHashes++;

			if (minedHashes > 1000000)
			{
				minedHashes = 0;
				totalBytesCleared = GC.GetAllocatedBytesForCurrentThread();
				GC.Collect();
			}
		}

		addingBlock.TimeOfMining = Time.realtimeSinceStartupAsDouble - addingBlock.TimeOfMining;
		addingBlock.HashrateInSeconds = (addingBlock.nonce / addingBlock.TimeOfMining) * 1000;
		BlockchainInfo.Add(addingBlock);

		Debug.Log(addingBlock.hashingDataWithNonce);
		Debug.Log("Bytes: " + totalBytesCleared);

		yield return null;
	}
}
