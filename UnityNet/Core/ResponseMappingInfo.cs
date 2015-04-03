using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace UnityNet.Core
{
    public class ResponseMappingInfo
    {
        private static ResponseMappingInfo instance;

        private ResponseMappingInfo()
        {

        }

        public static ResponseMappingInfo Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new ResponseMappingInfo();
                }
                return instance;
            }
        }

        private Dictionary<int, Type> responseMapping = new Dictionary<int, Type>();

        public void AddResponeMapping(int id, Type classType)
        {
            if (classType == null)
            {
                throw new ArgumentNullException("classType == null");
            }

            responseMapping.Add(id, classType);
        }

        public Type getResponseMapping(int id)
        {
            return responseMapping[id];
        }

        public object CreateResponseMapping(int id)
        {
            Type classType = responseMapping[id];
            if (classType != null)
            {
               return classType.Assembly.CreateInstance(classType.FullName);
            }

            return null;
        }
    }
}
