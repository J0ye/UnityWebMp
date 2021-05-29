using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Msg
{
    public class PositionMessage : IDMessage
    {
        public Vector3 position;

        public PositionMessage()
        {
            type = WebsocketMessageType.Position;
        }
        public PositionMessage(Vector3 pos)
        {
            type = WebsocketMessageType.Position;
            position = pos;
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
        /// Empty Constructor
        /// </summary>
        public TransformMessage(Transform target)
        {
            type = WebsocketMessageType.Transform;
            position = target.position;
            scale = target.localScale;
            rotation = target.rotation;
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
        /// Quick constructor. Converts target transform into message. Will also try to convert a string into Guid.
        /// It will throw an error, if it is unable to parse the string called it.
        /// </summary>
        /// <param name="id">Guid as a string for this message</param>
        /// <param name="target">Target values to convert to a message</param>
        public TransformMessage(string id, Transform target)
        {
            type = WebsocketMessageType.Transform;
            SetGuid(id);
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
            type = WebsocketMessageType.Transform;
            SetGuid(target.connectionID);
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

    public class SyncedGameObjectMessage : DoubleIDMessage
    {
        public string transformMessage;
        public TransformMessage transform = new TransformMessage();

        public SyncedGameObjectMessage(SyncedGameObject target)
        {
            type = WebsocketMessageType.SyncedGameObject;
            SetMessageGuid(target.guid.ToString());
            transform.SetGuid(connectionID);
            transform.position = target.transform.position;
            transform.scale = target.transform.localScale;
            transform.rotation = target.transform.rotation;
        }
        /// <summary>
        /// Quick constructor
        /// </summary>
        /// <param name=objectID">Id of this object</param>
        /// <param name="target">Transform to convert into a message</param>
        public SyncedGameObjectMessage( Guid objectID, Transform target)
        {
            type = WebsocketMessageType.SyncedGameObject;
            SetMessageGuid(objectID.ToString());
            transform.SetGuid(connectionID);
            transform.position = target.position;
            transform.scale = target.localScale;
            transform.rotation = target.rotation;
        }
        /// <summary>
        /// Standard constructor, that will convert target id to string.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="sca"></param>
        /// <param name="rot"></param>
        public SyncedGameObjectMessage(Guid id, Guid messageID, Vector3 pos, Vector3 sca, Quaternion rot)
        {
            type = WebsocketMessageType.SyncedGameObject;
            SetGuid(id.ToString());
            SetMessageGuid(messageID.ToString());
            transform.SetGuid(connectionID);
            transform.position = pos;
            transform.scale = sca;
            transform.rotation = rot;
        }

        /// <summary>
        /// Standard constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="sca"></param>
        /// <param name="rot"></param>
        public SyncedGameObjectMessage(string id, string gameID, Vector3 pos, Vector3 sca, Quaternion rot)
        {
            type = WebsocketMessageType.SyncedGameObject;
            SetGuid(id);
            SetMessageGuid(gameID);
            transform.SetGuid(connectionID);
            transform.position = pos;
            transform.scale = sca;
            transform.rotation = rot;
        }
        /// <summary>
        /// Quick constructor. Converts target transform into message)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="target"></param>
        public SyncedGameObjectMessage(Guid id, Guid gameID, Transform target)
        {
            type = WebsocketMessageType.SyncedGameObject;
            SetGuid(id.ToString());
            SetMessageGuid(gameID.ToString());
            transform.SetGuid(connectionID);
            transform.position = target.position;
            transform.scale = target.localScale;
            transform.rotation = target.rotation;
        }

        /// <summary>
        /// Quick constructor. Converts target transform into message. Will also try to convert a string into Guid.
        /// It will throw an error, if it is unable to parse the string called it.
        /// </summary>
        /// <param name="id">Guid as a string for this message</param>
        /// <param name="target">Target values to convert to a message</param>
        public SyncedGameObjectMessage(string id, string gameID, Transform target)
        {
            type = WebsocketMessageType.SyncedGameObject;
            SetGuid(id);
            SetMessageGuid(gameID);
            transform.SetGuid(connectionID);
            transform.position = target.position;
            transform.scale = target.localScale;
            transform.rotation = target.rotation;
        }

        public static new SyncedGameObjectMessage FromJson(string target)
        {
            SyncedGameObjectMessage returnValue = JsonUtility.FromJson<SyncedGameObjectMessage>(target);
            returnValue.transform = TransformMessage.FromJson(returnValue.transformMessage);
            return returnValue;
        }

        public override string ToJson()
        {
            transformMessage = transform.ToJson();
            return JsonUtility.ToJson(this);
        }
    }
}
