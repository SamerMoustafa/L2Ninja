﻿using System.Collections.Generic;
using L2dotNET.GameService.Model.Items;
using L2dotNET.GameService.Model.Playable;
using L2dotNET.GameService.Model.Player;
using L2dotNET.GameService.Network.Serverpackets;

namespace L2dotNET.GameService.Network.Clientpackets.PetAPI
{
    class RequestGetItemFromPet : GameServerNetworkRequest
    {
        private int objectId;
        private long count;
        private int equipped;

        public RequestGetItemFromPet(GameClient client, byte[] data)
        {
            makeme(client, data);
        }

        public override void read()
        {
            objectId = readD();
            count = readQ();
            equipped = readD();
        }

        public override void run()
        {
            L2Player player = Client.CurrentPlayer;

            if ((player.Summon == null) || !(player.Summon is L2Pet) || (player.EnchantState != 0))
            {
                player.sendActionFailed();
                return;
            }

            L2Pet pet = (L2Pet)player.Summon;

            if (pet.Dead)
            {
                player.sendSystemMessage(SystemMessage.SystemMessageId.CANNOT_GIVE_ITEMS_TO_DEAD_PET);
                player.sendActionFailed();
                return;
            }

            //if (!pet.Inventory.Items.Contains(objectId))
            //{
            //    player.sendActionFailed();
            //    return;
            //}

            L2Item item = pet.Inventory.Items[objectId];

            if (item.TempBlock)
            {
                player.sendActionFailed();
                return;
            }

            if ((item.Template.is_drop == 0) || (item.Template.is_destruct == 0) || (item.Template.is_trade == 0) || (item.Template.can_equip_hero != -1) || (pet.ControlItem.ObjID == objectId))
            {
                player.sendSystemMessage(SystemMessage.SystemMessageId.ITEM_NOT_FOR_PETS);
                player.sendActionFailed();
                return;
            }

            if (count < 0)
                count = 1;
            else if (count > item.Count)
                count = item.Count;

            List<long[]> items = new List<long[]>();
            items.Add(new[] { objectId, count });
            //pet.Inventory.transferFrom(player, items, true);
        }
    }
}