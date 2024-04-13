namespace ImprovedClothing
{
    public class Main : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("Improved Clothing is online!");
            Settings.OnLoad();
        }

        public override void OnUpdate()
        {

            //take off shoes
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.settings.footwearKey))
            {
                Utils.takeOffOutermostClothingItem(ClothingRegion.Feet);
            }

            //take off coat
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.settings.coatKey))
            {
                Utils.takeOffOutermostClothingItem(ClothingRegion.Chest);
            }

            //take off gloves
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.settings.glovesKey))
            {
                Utils.takeOffOutermostClothingItem(ClothingRegion.Hands);
            }

            //take off hat
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.settings.hatKey))
            {
                Utils.takeOffOutermostClothingItem(ClothingRegion.Head);
            }

            //take off pants
            if (InputManager.GetKeyDown(InputManager.m_CurrentContext, Settings.settings.pantsKey))
            {
                Utils.takeOffOutermostClothingItem(ClothingRegion.Legs);
            }
        }

    }
}
