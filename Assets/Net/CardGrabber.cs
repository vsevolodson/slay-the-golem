using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Net
{
    public sealed class CardGrabber : NetworkBehaviour
    {
        private NetworkIdentity _held;
        private bool _hadAuthority;

        private void Update()
        {
            if (!isLocalPlayer)
                return;

            var pointer = Pointer.current;
            if (pointer == null || Camera.main == null)
                return;

            var world = Camera.main.ScreenToWorldPoint(pointer.position.ReadValue());

            if (pointer.press.wasPressedThisFrame)
                TryGrab(world);

            DragHeldCard(world);

            if (pointer.press.wasReleasedThisFrame && _held != null)
            {
                CmdRelease(_held);

                _held = null;
                _hadAuthority = false;
            }
        }

        private void TryGrab(Vector3 world)
        {
            var hit = Physics2D.OverlapPoint(world);
            if (hit == null)
                return;

            var card = hit.GetComponent<NetworkIdentity>();
            if (card == null)
                return;

            _held = card;
            _hadAuthority = false;

            CmdGrab(card);
        }

        private void DragHeldCard(Vector3 world)
        {
            if (_held == null)
                return;

            if (_held.isOwned)
            {
                _hadAuthority = true;
                _held.transform.position = new Vector3(world.x, world.y, 0f);

                return;
            }

            if (_hadAuthority)
            {
                _held = null;
                _hadAuthority = false;
            }
        }

        [Command]
        private void CmdGrab(NetworkIdentity card)
        {
            if (card == null)
                return;

            var networkCard = card.GetComponent<NetworkCard>();
            if (networkCard == null || networkCard.HolderNetId == netId)
                return;

            if (networkCard.IsHeld)
                card.RemoveClientAuthority();

            networkCard.SetHolder(netId);
            card.AssignClientAuthority(connectionToClient);
        }

        [Command]
        private void CmdRelease(NetworkIdentity card)
        {
            if (card == null)
                return;

            var networkCard = card.GetComponent<NetworkCard>();
            if (networkCard == null || networkCard.HolderNetId != netId)
                return;

            networkCard.SetHolder(0);
            card.RemoveClientAuthority();
        }
    }
}
