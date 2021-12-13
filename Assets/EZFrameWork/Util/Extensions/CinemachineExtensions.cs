using Cinemachine;

namespace EZFramework
{

    public static class CinemachineExtensions
    {

        public static float PositionInDistance(this CinemachineDollyCart dollyCart)
        {
            if (dollyCart.m_Path == null)
                return 0;

            return dollyCart.m_Path.PathLength * dollyCart.m_Position;
        }

    }
}
