### Unity Blockchain Mining Script

This unique script implements mining directly within Unity!

The code provides almost complete blockchain functionality with mining. However, information distribution and the database itself are not included. The code is intended for understanding the workings of mining, its structure, and functionality.

To use this script, create an empty game object, attach the script to it, and fill in any text in the "Data" field. In the "Difficulty" field, input lowercase hexadecimal characters. This defines the rule for the mined block. For example, entering zeros (e.g., "00") is akin to the Bitcoin system. Now press the "Mine" button to generate a mined block.

Wait a moment, and a new entry will appear in the Blockchain Info, consisting of:

- Block (block number)
- Nonce (random number required for block mining)
- Data (data you entered in the "Data" field)
- Previous Hash (blocks in the blockchain are connected via the previous hash)
- Hash (hashed according to the rule)
- Time Of Mining (mining time, specified in seconds)
- Hashrate In Seconds (your computer's hashrate per second, i.e., the number of hashes attempted per second)

After mining, a new entry will appear in the Console tab. This entry is the one fed into the SHA256 algorithm to find the desired hash based on the rule. You can copy this data string and paste it into an online SHA256 calculation service (e.g., [online SHA256 calculator](https://emn178.github.io/online-tools/sha256.html)) to verify the mining algorithm's operation. For example, if you set the Difficulty field to "00," pressed the Mine button, and the Console displayed "1The Lobos Robotics NFT. All rights reserved.01" (do not copy quotes), inputting this string into the online tool should yield "00b93e033b278155946fc451304a2aec970fc9fc01401b4897f2cd913aec975f." This demonstrates the mining rule that computes a hash with two leading zeros.
