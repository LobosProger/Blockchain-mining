using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using System.Security.Cryptography;

public class Blockchain : MonoBehaviour
{
	[SerializeField]
	private string data;
	[SerializeField]
	private string difficulty;


	[Space(20)]
	public List<Block> blockchain = new List<Block>();

	[SerializeField]
	private bool mine;
	private void Start()
	{
		if (blockchain.Count == 0)
		{
			Block genesisBlock = new Block(0, "Genesis block", "0");
			genesisBlock.mineBlock();
			genesisBlock.assignDataToBlock();

			blockchain.Add(genesisBlock);
		}
	}

	private void Update()
	{
		if (mine)
		{
			mine = false;
			StartCoroutine(Mining());
		}
	}

	IEnumerator Mining()
	{
		Block miningBlock = new Block(blockchain.Count, data, blockchain[blockchain.Count - 1].validHash);
		miningBlock.mineBlock();

		while (!hasStringBuilderSubset(miningBlock.validHashStringBuilder, difficulty))
		{
			for (int i = 0; i <= 200000; i++)
			{
				if (hasStringBuilderSubset(miningBlock.validHashStringBuilder, difficulty))
					break;
				else
					miningBlock.mineBlock();
			}

			Debug.Log("Mining...");
			GC.Collect();
			yield return Resources.UnloadUnusedAssets();
			yield return new WaitForSecondsRealtime(0.05f);
		}

		Debug.Log("Block has been mined!");
		miningBlock.assignDataToBlock();
		blockchain.Add(miningBlock);
		yield return null;
	}

	private bool hasStringBuilderSubset(StringBuilder builder, string subset)
	{
		int length = subset.Length;
		bool hasSubset = true;

		for (int i = 0; i < length; i++)
		{
			if (builder[i] != subset[i])
			{
				hasSubset = false;
				break;
			}
		}

		return hasSubset;
	}
}

[System.Serializable]
public class Block
{
	public int block;
	public string timestamp;
	public string data;
	public string previousHash;

	[Space]
	public string hashRoot;

	[Space(20)]
	public string validHash;
	public StringBuilder validHashStringBuilder;
	public ulong nonce = 0;

	[Space(70)]
	public string connectedData;
	public string hashRootWithNonce;
	private StringBuilder connectedHashRootWithNonceForMining = new StringBuilder();

	[Space]
	public double timeOfMining;

	public Block(int block, string data, string previousHash)
	{
		this.block = block;
		this.timestamp = Convert.ToString(DateTime.Now);
		this.data = data;
		this.previousHash = previousHash;

		connectedData = string.Concat(block, timestamp, data, previousHash);
		hashRoot = createSHA256(connectedData).ToString();

		connectedHashRootWithNonceForMining = new StringBuilder();
		this.timeOfMining = Time.realtimeSinceStartupAsDouble;
	}

	public void mineBlock()
	{
		nonce++;
		connectedHashRootWithNonceForMining.Clear();
		connectedHashRootWithNonceForMining.Append(hashRoot);
		connectedHashRootWithNonceForMining.Append(nonce);
		validHashStringBuilder = createSHA256(connectedHashRootWithNonceForMining.ToString());
	}

	public void assignDataToBlock()
	{
		validHash = validHashStringBuilder.ToString();
		hashRootWithNonce = connectedHashRootWithNonceForMining.ToString();
		timeOfMining = Time.realtimeSinceStartupAsDouble - timeOfMining;
	}



	private StringBuilder createSHA256(string Data)
	{
		SHA256 sha256 = new SHA256CryptoServiceProvider();
		sha256.ComputeHash(ASCIIEncoding.ASCII.GetBytes(Data));
		byte[] result = sha256.Hash;
		StringBuilder strBuilder = new StringBuilder();
		for (int i = 0; i < result.Length; i++)
		{
			strBuilder.Append(result[i].ToString("x2"));
		}
		return strBuilder;
	}
}