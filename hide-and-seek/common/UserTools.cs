using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hide_and_seek.common
{
    public class UserTools
    {
        /// <summary>
        /// 获取entityid
        /// </summary>
        /// <param name="pf"></param>
        /// <returns></returns>
        public static int GetEntityPlatformUserIdentifierAbs(PlatformUserIdentifierAbs pf)
        {
            try
            {
                foreach (ClientInfo _info in ConnectionManager.Instance.Clients.List)
                {
                    if (_info.PlatformId.ReadablePlatformUserIdentifier.Equals(pf.ReadablePlatformUserIdentifier))
                    {
                        return _info.entityId;
                    }
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
        }


        /// <summary>
        /// 获取ClientInfo
        /// </summary>
        /// <param name="_EntityId"></param>
        /// <returns></returns>
        public static ClientInfo GetClientInfoFromEntityId(int _EntityId)
        {
            ClientInfo _cInfo = ConnectionManager.Instance.Clients.ForEntityId(_EntityId);
            if (_cInfo != null)
            {
                return _cInfo;
            }
            return null;
        }
    }


}
