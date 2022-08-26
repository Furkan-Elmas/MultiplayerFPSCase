using Mirror;

public class RespawnHandle : NetworkBehaviour
{
    [Command]
    public void PlayerRespawn()
    {
        if (!isLocalPlayer)
            return;

        NetworkManagerLobby room = NetworkManager.singleton as NetworkManagerLobby;
        Destroy(gameObject);
    }
}
