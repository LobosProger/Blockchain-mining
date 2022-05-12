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

		public string MineTheBlock(ulong Nonce)
		{
			hashingDataWithNonce = connectedData + Nonce.ToString();
			return EncryptSHA256(hashingDataWithNonce);
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

	private ulong rightNonce = 0;
	private string rightHash;

	Thread t1; private ulong nonce1 = 0; private ulong nonce1 = 0; private string hash1;
	Thread t2; private ulong nonce2 = 0; private string hash2;
	Thread t3; private ulong nonce3 = 0; private string hash3;
	Thread t4; private ulong nonce4 = 0; private string hash4;

	private void Start()
	{
		t1 = new Thread(MineMultiThread1);
		t2 = new Thread(MineMultiThread2);
		t3 = new Thread(MineMultiThread3);
		t4 = new Thread(MineMultiThread4);

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

				if (!t1.IsAlive)
				{
					t1.Start();
					t2.Start();
					t3.Start();
					t4.Start();
				}

				/*localNonce += 200000;

				Thread t2 = new Thread(MineMultiThread);
				t2.Start();*/


				/*for (int i = 1; i <= 200000; i++)
				{
					if (currentMiningBlock.Hash.Substring(0, currentAmountDifficulty) == difficulty)
					{
						Mine = false;
						StartedMining = false;

						localNonce = 1;
						break;
					}
					else
						currentMiningBlock.MineTheBlock();
				}*/

				/*if (!Mine)
				{
					
				}*/


			}
		}

		if (rightNonce != 0)
		{
			rightNonce = 0;
			FinishedMining();
		}
	}

	private IEnumerator ClearNonAlloc()
	{
		ClearedGarbage = false;
		GC.Collect();
		GC.WaitForPendingFinalizers();
		yield return new WaitForSecondsRealtime(1f);

		if (Mine)
			Debug.Log("Hashrate: " + (currentMiningBlock.nonce / (Time.timeAsDouble - currentMiningBlock.TimeOfMining)) * 1000 + " H/s");
		ClearedGarbage = true;

		yield return new WaitForSecondsRealtime(1f);
		StartCoroutine(ClearNonAlloc());
	}

	private void MineMultiThread1() => ProccessOfMining(1);
	private void MineMultiThread2() => ProccessOfMining(2);
	private void MineMultiThread3() => ProccessOfMining(3);
	private void MineMultiThread4() => ProccessOfMining(4);

	private void ProccessOfMining(int x)
	{
		ulong startNonce, maxNonce;

		if (x == 1)
		{
			startNonce = 0;
			maxNonce = 200000;
		}
		if (x == 2)
		{
			startNonce = 200000;
			maxNonce = 400000;
		}
		if (x == 3)
		{
			startNonce = 400000;
			maxNonce = 600000;
		}
		if (x == 4)
		{
			startNonce = 600000;
			maxNonce = 800000;
		}

		while (true)
		{
			for (ulong i = startNonce; i <= maxNonce; i++)
			{
				if (rightNonce == 0 && Mine && currentMiningBlock.Hash.Substring(0, currentAmountDifficulty) == difficulty)
				{
					currentMiningBlock.nonce = i;
					rightNonce = i;
					Mine = false;
					break;
				}
				else if (!Mine)
					break;
				else
					currentMiningBlock.MineTheBlock(i);
			}

			while (!Mine || !ClearedGarbage || !StartedMining) { }
		}
	}

	private void FinishedMining()
	{
		Mine = false;
		StartedMining = false;

		currentMiningBlock.TimeOfMining = Time.timeAsDouble - currentMiningBlock.TimeOfMining;
		currentMiningBlock.HashrateInSeconds = (currentMiningBlock.nonce / currentMiningBlock.TimeOfMining) * 1000;
		BlockchainInfo.Add(currentMiningBlock);

		Debug.Log(currentMiningBlock.hashingDataWithNonce);
	}
}
