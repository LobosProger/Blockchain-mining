using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;
using MyBox;
using NaughtyAttributes;
using System.Security.Cryptography;
using System.Text;
public class Blockchain : MonoBehaviour
{
	public string difficulty = "0";
	[ResizableTextArea]
	public string Data;
	public List<Block> BlockchainInfo = new List<Block>();

	private Block currentMiningBlock;
	private int currentAmountDifficulty;

	[Serializable]
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

		[HideInInspector]
		public string connectedData;
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

		public string MineTheBlock(ulong Nonce)
		{
			return EncryptSHA256(connectedData + Nonce.ToString());
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

	[SerializeField]
	private bool Mine;
	private bool StartedMining;
	private bool ClearedGarbage = true;

	Queue<ValidHash> MinedHashes = new Queue<ValidHash>();

	private const ulong maxNonces = 300000;
	private MiningThread t1 = new MiningThread { nonce = 1, maxNonce = 300001, NumberOfThread = 1 };
	private MiningThread t2 = new MiningThread { nonce = 10000000, maxNonce = 10300000, NumberOfThread = 2 };
	private MiningThread t3 = new MiningThread { nonce = 20000000, maxNonce = 20300000, NumberOfThread = 3 };

	private void Start()
	{
		t1.thread = new Thread(MineMultiThread1);
		t2.thread = new Thread(MineMultiThread2);
		t3.thread = new Thread(MineMultiThread3);

		StartCoroutine(ClearNonAlloc());
	}
	private void Update()
	{
		if (ClearedGarbage)
		{
			if (Mine)
			{
				if (!StartedMining)
				{
					currentMiningBlock.AssignDataToBlock(Data, BlockchainInfo);
					currentMiningBlock.TimeOfMining = Time.timeAsDouble;
					currentAmountDifficulty = difficulty.Length;

					StartedMining = true;
				}

				if (!t1.thread.IsAlive)
				{
					t1.thread.Start();
					t2.thread.Start();
					t3.thread.Start();
				}
			}
		}

		if (MinedHashes.Count > 0)
			StartCoroutine(FinishedMining());
	}

	private IEnumerator ClearNonAlloc()
	{
		ClearedGarbage = false;
		GC.Collect();
		GC.WaitForPendingFinalizers();

		yield return new WaitForSeconds(0.2f);
		ClearedGarbage = true;

		yield return new WaitForSeconds(1f);
		if (Mine)
			Debug.Log("Mining...");
		StartCoroutine(ClearNonAlloc());
	}

	private void MineMultiThread1() => ProccessOfMining(t1);
	private void MineMultiThread2() => ProccessOfMining(t2);
	private void MineMultiThread3() => ProccessOfMining(t3);

	private void ProccessOfMining(MiningThread miningThread)
	{
		ulong startNonce = miningThread.nonce;
		ulong maxNonce = miningThread.maxNonce;
		string miningHash = "";
		bool resetValues = false;

		while (true)
		{
			startNonce = startNonce + maxNonces;
			maxNonce = maxNonce + maxNonces;
			resetValues = false;

			for (ulong i = startNonce; i <= maxNonce; i++)
			{
				if (MinedHashes.Count == 0 && Mine && miningHash.Length >= currentAmountDifficulty && miningHash.Substring(0, currentAmountDifficulty) == difficulty)
				{
					ValidHash validHash = new ValidHash { Hash = miningHash, nonce = i - 1, ThreadWhichMined = miningThread.NumberOfThread };
					MinedHashes.Enqueue(validHash);

					Mine = false;
					StartedMining = false;

					startNonce = miningThread.nonce;
					maxNonce = miningThread.maxNonce;
					miningHash = "";

					break;
				}
				else if (!Mine || MinedHashes.Count > 0)
				{
					Mine = false;
					StartedMining = false;

					startNonce = miningThread.nonce;
					maxNonce = miningThread.maxNonce;
					miningHash = "";

					break;
				}
				else
					miningHash = currentMiningBlock.MineTheBlock(i);
			}

			while (!Mine || !ClearedGarbage || !StartedMining)
			{
				if (!Mine)
				{
					if (!resetValues)
					{
						resetValues = true;

						startNonce = miningThread.nonce;
						maxNonce = miningThread.maxNonce;
						miningHash = ""; //! Чтоб по миллион раз не выставлять значения (для оптимизации)
					}
				}
			}
		}
	}

	private IEnumerator FinishedMining()
	{
		yield return new WaitForEndOfFrame();
		Mine = false;
		StartedMining = false;

		if (MinedHashes.Count > 0)
		{
			ValidHash hash = MinedHashes.Dequeue();
			currentMiningBlock.nonce = hash.nonce;
			currentMiningBlock.Hash = hash.Hash;
			currentMiningBlock.hashingDataWithNonce = currentMiningBlock.connectedData + hash.nonce.ToString();

			currentMiningBlock.TimeOfMining = Time.timeAsDouble - currentMiningBlock.TimeOfMining;
			currentMiningBlock.HashrateInSeconds = (currentMiningBlock.nonce / currentMiningBlock.TimeOfMining) * 1000;
			BlockchainInfo.Add(currentMiningBlock);

			Debug.Log("Block mined!\n" + currentMiningBlock.hashingDataWithNonce);
			Debug.Log("Number of thread, which mined: " + hash.ThreadWhichMined);
		}
	}

	public struct MiningThread
	{
		public Thread thread;
		public ulong nonce;
		public ulong maxNonce;

		public int NumberOfThread;
	}

	public struct ValidHash
	{
		public string Hash;
		public ulong nonce;

		public int ThreadWhichMined;
	}
}
