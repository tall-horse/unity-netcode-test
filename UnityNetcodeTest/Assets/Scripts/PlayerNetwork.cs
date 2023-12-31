using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    [SerializeField] private Transform spawnedObjectPrefab;
    private Transform spawnedObjectTransform;
  private NetworkVariable<MyCustomData> randomNumber = new NetworkVariable<MyCustomData>(
    new MyCustomData
    {
      _int = 56,
      _bool = true
    },
        NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

  public struct MyCustomData : INetworkSerializable
  {
    public int _int;
    public bool _bool;
    public FixedString128Bytes message;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
      serializer.SerializeValue(ref _int);
      serializer.SerializeValue(ref _bool);
      serializer.SerializeValue(ref message);
    }
  }
  public override void OnNetworkSpawn()
  {
    randomNumber.OnValueChanged += (MyCustomData previousValue, MyCustomData newValue) =>
    {
      Debug.Log(OwnerClientId + ";  " + newValue._int + "; " + newValue._bool + "; " + newValue.message);
    };
  }
  // Update is called once per frame
  void Update()
  {

    if (!IsOwner)
      return;

    if (Input.GetKeyDown(KeyCode.T))
    {
        spawnedObjectTransform = Instantiate(spawnedObjectPrefab);
        spawnedObjectPrefab.GetComponent<NetworkObject>().Spawn(true);
        //TestClientRPC(new ClientRpcParams { Send = new ClientRpcSendParams { TargetClientIds = new List<ulong> {1} }});
    //   randomNumber.Value = new MyCustomData
    //   {
    //     _int = 10,
    //     _bool = false,
    //     message = "Test message",
    //   };
    }

    if(Input.GetKeyDown(KeyCode.Y))
    {
        spawnedObjectPrefab.GetComponent<NetworkObject>().Despawn(true);
        Destroy(spawnedObjectTransform.gameObject);
    }

    Vector3 moveDir = new Vector3(0, 0, 0);
    if (Input.GetKey(KeyCode.W)) moveDir.z = +1f;
    if (Input.GetKey(KeyCode.S)) moveDir.z = -1f;
    if (Input.GetKey(KeyCode.A)) moveDir.x = -1f;
    if (Input.GetKey(KeyCode.D)) moveDir.x = +1f;

    float moveSpeed = 3f;
    transform.position += moveDir * moveSpeed * Time.deltaTime;
  }
    [ServerRpc]
  private void TestServerRPC(ServerRpcParams serverRpcParams)
  {
    Debug.Log("TestServerRPC" + " " + OwnerClientId + "; " + serverRpcParams.Receive.SenderClientId);
  }

  [ClientRpc]
  private void TestClientRPC(ClientRpcParams clientRpcParams)
  {
    Debug.Log("TestClientRPC");
  }
}
