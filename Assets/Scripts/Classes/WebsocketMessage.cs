using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Msg
{
    public enum WebsocketMessageType { Request, Position, Chat, ID, SyncString, SyncFloat, RPC, Transform, SyncedGameObject, VRPlayer }
    public class WebsocketMessage
    {
        public WebsocketMessageType type;

        public static WebsocketMessage FromJson(string target)
        {
            return JsonUtility.FromJson<WebsocketMessage>(target);
        }

        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public class WebsocketRequest : IDMessage
    {
        public WebsocketMessageType requestType;

        public WebsocketRequest()
        {
            connectionID = Guid.NewGuid().ToString();
            type = WebsocketMessageType.Request;
        }

        public WebsocketRequest(WebsocketMessageType request, Guid id)
        {
            connectionID = id.ToString();
            type = WebsocketMessageType.Request;
            requestType = request;
        }

        public new static WebsocketRequest FromJson(string target)
        {
            return JsonUtility.FromJson<WebsocketRequest>(target);
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public class IDMessage : WebsocketMessage
    {
        /// <summary>
        /// This is used to sign the message with an id unique to this instance of the system and its connection to the server
        /// </summary>
        public string connectionID;
        /// <summary>
        /// Id as a System.Guid
        /// </summary>
        public System.Guid Guid { get => System.Guid.Parse(connectionID); }

        public IDMessage()
        {
            type = WebsocketMessageType.ID;
        }

        public IDMessage(string guid)
        {
            type = WebsocketMessageType.ID;
            SetGuid(guid);
        }

        public static new IDMessage FromJson(string target)
        {
            return JsonUtility.FromJson<IDMessage>(target);
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void SetGuid(string target)
        {
            if (Guid.TryParse(target, out Guid temp))
            {
                connectionID = target;
            }
            else
            {
                throw new Exception("Script is trying to set the id of a IDMessage to an invalid guid");
            }
        }
    }

    /// <summary>
    /// Extends the IDMessage by another member for another ID, for example a game-specific ID.
    /// </summary>
    public class DoubleIDMessage : IDMessage
    {
        public string messageGuid;

        public DoubleIDMessage()
        {
            type = WebsocketMessageType.ID;
        }

        public DoubleIDMessage(string guid, string gameGuid)
        {
            type = WebsocketMessageType.ID;
            SetGuid(guid);
            SetMessageGuid(gameGuid);
        }

        public static new DoubleIDMessage FromJson(string target)
        {
            return JsonUtility.FromJson<DoubleIDMessage>(target);
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public void SetMessageGuid(string target)
        {
            messageGuid = target;
        }
    }

    public class SyncVarMessage : IDMessage
    {
        public string callName;
        public string stringValue;
        public float floatValue;

        public SyncVarMessage(WebsocketMessageType targetType, Guid id, string callName, string stringValue, float floatValue)
        {
            if (targetType != WebsocketMessageType.SyncFloat && targetType != WebsocketMessageType.SyncString)
                throw new Exception("Script is trying to create a SyncVarMessage for " + callName + " without the proper type");

            this.type = targetType;
            this.connectionID = id.ToString();
            this.callName = callName;
            this.stringValue = stringValue;
            this.floatValue = floatValue;
        }
    }

    public class RPCMessage : IDMessage
    {
        public string procedureGuid;
        public string procedureName;

        public RPCMessage(Guid gameID, Guid procedureId, string procedure)
        {
            type = WebsocketMessageType.RPC;
            connectionID = gameID.ToString();
            procedureGuid = procedureId.ToString();
            procedureName = procedure;
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
        public static new RPCMessage FromJson(string target)
        {
            return JsonUtility.FromJson<RPCMessage>(target);
        }
    }
}
