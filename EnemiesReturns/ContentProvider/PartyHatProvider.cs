using RoR2.ContentManagement;
using System.Linq;
using UnityEngine;

namespace EnemiesReturns
{
    public partial class ContentProvider : IContentPackProvider
    {
        private void CreatePartyHat(GameObject[] assets)
        {
            if (Items.PartyHat.PartyHatFactory.ShouldThrowParty())
            {
                var partyHatFactory = new Items.PartyHat.PartyHatFactory();
                Items.PartyHat.PartyHatFactory.PartyHatDisplay = partyHatFactory.SetupDisplayPrefab(assets.First(assets => assets.name == "ReturnsPartyHat"));
                Content.Items.PartyHat = partyHatFactory.CreateItem();

                itemList.Add(Content.Items.PartyHat);
            }
        }
    }
}
