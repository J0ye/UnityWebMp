using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Msg
{
    public enum WebsocketMessageType { Request, Position, Chat, ID, SyncString, SyncFloat, RPC, Transform }
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
            guid = Guid.NewGuid().ToString();
            type = WebsocketMessageType.Request;
        }

        public WebsocketRequest(WebsocketMessageType request, Guid id)
        {
            guid = id.ToString();
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
        public string guid;
        public System.Guid Guid { get => System.Guid.Parse(guid); }

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
                guid = target;
            }
            else
            {
                throw new Exception("Script is trying to set the id of a PositionMessage to an invalid guid");
            }
        }
    }

    public class PositionMessage : IDMessage
    {
        public Vector3 position;

        public PositionMessage()
        {
            type = WebsocketMessageType.Position;
        }
        public PositionMessage(Guid id, Vector3 pos)
        {
            type = WebsocketMessageType.Position;
            SetGuid(id.ToString());
            position = pos;
        }

        public PositionMessage(string id, Vector3 pos)
        {
            type = WebsocketMessageType.Position;
            SetGuid(id);
            position = pos;
        }

        public static new PositionMessage FromJson(string target)
        {
            return JsonUtility.FromJson<PositionMessage>(target);
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this);
        }
    }

    public class TransformMessage : IDMessage
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;

        public static explicit operator TransformMessage(PositionMessage p) => new TransformMessage(p);

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public TransformMessage()
        {
            type = WebsocketMessageType.Transform;
        }
        /// <summary>
        /// Standard constructor, that will convert target id to string.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="sca"></param>
        /// <param name="rot"></param>
        public TransformMessage(Guid id, Vector3 pos, Vector3 sca, Quaternion rot)
        {
            type = WebsocketMessageType.Transform;
            SetGuid(id.ToString());
            position = pos;
            scale = sca;
            rotation = rot;
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="sca"></param>
        /// <param name="rot"></param>
        public TransformMessage(string id, Vector3 pos, Vector3 sca, Quaternion rot)
        {
            type = WebsocketMessageType.Transform;
            SetGuid(id);
            position = pos;
            scale = sca;
            rotation = rot;
        }
        /// <summary>
        /// Quick constructor. Converts target transform into message)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="target"></param>
        public TransformMessage(Guid id, Transform target)
        {
            type = WebsocketMessageType.Transform;
            SetGuid(id.ToString());
            position = target.position;
            scale = target.localScale;
            rotation = target.rotation;
        }

        /// <summary>
        /// Quick constructor to parse PositionMessage into TransformMessage
        /// </summary>
        /// <param name="target"></param>
        public TransformMessage(PositionMessage target)
        {
            type = WebsocketMessageType.Transform;
            SetGuid(target.guid);
            position = target.position;
            scale = new Vector3(-9999, -9999, -9999);
            rotation = new Quaternion(-9999, -9999, -9999, -9999);
        }

        public static new TransformMessage FromJson(string target)
        {
            return JsonUtility.FromJson<TransformMessage>(target);
        }

        public override string ToJson()
        {
            return JsonUtility.ToJson(this);
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
            this.guid = id.ToString();
            this.callName = callName;
            this.stringValue = stringValue;
            this.floatValue = floatValue;
        }
    }

    public class RPCMessage : IDMessage
    {
        public string procedureGuid;
        public string procedureName;

        public RPCMessage(Guid playerId, Guid procedureId, string procedure)
        {
            type = WebsocketMessageType.RPC;
            guid = playerId.ToString();
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
