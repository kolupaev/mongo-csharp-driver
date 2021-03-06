﻿/* Copyright 2010 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;

namespace MongoDB.Driver.Internal {
    internal class MongoReplyMessage<TDocument> : MongoMessage {
        #region private fields
        private ResponseFlags responseFlags;
        private long cursorId;
        private int startingFrom;
        private int numberReturned;
        private List<TDocument> documents;
        #endregion

        #region constructors
        internal MongoReplyMessage()
            : base(MessageOpcode.Reply) {
        }
        #endregion

        #region internal properties
        internal ResponseFlags ResponseFlags {
            get { return responseFlags; }
        }

        internal long CursorId {
            get { return cursorId; }
        }

        internal int StartingFrom {
            get { return startingFrom; }
        }

        internal int NumberReturned {
            get { return numberReturned; }
        }

        internal List<TDocument> Documents {
            get { return documents; }
        }
        #endregion

        #region internal methods
        internal void ReadFrom(
            BsonBuffer buffer
        ) {
            var messageStartPosition = buffer.Position;

            ReadMessageHeaderFrom(buffer);
            responseFlags = (ResponseFlags) buffer.ReadInt32();
            cursorId = buffer.ReadInt64();
            startingFrom = buffer.ReadInt32();
            numberReturned = buffer.ReadInt32();
            documents = new List<TDocument>();

            BsonReader bsonReader = BsonReader.Create(buffer);
            if ((responseFlags & ResponseFlags.CursorNotFound) != 0) {
                throw new MongoQueryException("Cursor not found.");
            }
            if ((responseFlags & ResponseFlags.QueryFailure) != 0) {
                var document = BsonDocument.ReadFrom(bsonReader);
                var err = document["$err", null].AsString ?? "Unknown error.";
                var message = string.Format("QueryFailure flag set: {0} (response: {1})", err, document.ToJson());
                throw new MongoQueryException(message);
            }

            while (buffer.Position - messageStartPosition < messageLength) {
                var document = BsonSerializer.Deserialize<TDocument>(bsonReader);
                documents.Add(document);
            }
        }
        #endregion
    }
}
