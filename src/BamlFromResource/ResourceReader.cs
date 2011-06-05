namespace Reflector.BamlViewer
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Globalization;
	using System.Reflection;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text;

	internal class ResourceReader
	{
		private Resource[] resources;

		public ResourceReader(Stream stream)
		{
			BinaryReader reader = new BinaryReader(stream);

			uint signature = reader.ReadUInt32();
			if (signature != 0xbeefcace)
			{
				throw new InvalidOperationException("Invalid resource file signature.");
			}

			int headerVersion = reader.ReadInt32();
			if (headerVersion > 1)
			{
				reader.BaseStream.Seek((long) reader.ReadInt32(), SeekOrigin.Current);
			}
			else
			{
				reader.ReadInt32(); // skip
				
				string resourceReaderType = reader.ReadString();
				string resourceSetType = reader.ReadString();

				if (!resourceReaderType.StartsWith("System.Resources.ResourceReader"))
				{
					throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "This resource reader type is not supported: '{0}'.", resourceReaderType));
				}
			}

			int runtimeVersion = reader.ReadInt32();
			if ((runtimeVersion != 2) && (runtimeVersion != 1))
			{
				throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, "Unknown runtime version \'{0}\'.", runtimeVersion));
			}

			int resourceCount = reader.ReadInt32();

			string[] resourceTypeNames = new string[reader.ReadInt32()];
			for (int i = 0; i < resourceTypeNames.Length; i++)
			{
				resourceTypeNames[i] = reader.ReadString();
			}

			if ((reader.BaseStream.Position % 8) != 0)
			{
				reader.ReadBytes((int) (8 - (reader.BaseStream.Position % 8)));
			}

			int[] resourceNameHashValues = new int[resourceCount];
			for (int i = 0; i < resourceNameHashValues.Length; i++)
			{
				resourceNameHashValues[i] = reader.ReadInt32();
			}

			int[] resourceNamePositions = new int[resourceCount];
			for (int i = 0; i < resourceNamePositions.Length; i++)
			{
				resourceNamePositions[i] = reader.ReadInt32();
			}

			long resourceValueSectionOffset = reader.ReadInt32();
			long resourceNameSectionOffset = reader.BaseStream.Position;

			int[] resourceValuePositions = new int[resourceCount];
			string[] resourceNames = new string[resourceCount];
			for (int i = 0; i < resourceNames.Length; i++)
			{
				reader.BaseStream.Position = resourceNameSectionOffset + resourceNamePositions[i];
				int count = this.Read7BitEncodedInt(reader);
				byte[] bytes = reader.ReadBytes(count);
				resourceNames[i] = Encoding.Unicode.GetString(bytes, 0, count);
				resourceValuePositions[i] = reader.ReadInt32();
			}

			this.resources = new Resource[resourceCount];
			for (int i = 0; i < this.resources.Length; i++)
			{
				reader.BaseStream.Position = resourceValueSectionOffset + resourceValuePositions[i];

				Resource resource = new Resource();
				resource.Name = resourceNames[i];
				resource.Value = null;

				switch (runtimeVersion)
				{
					case 1:
						this.LoadResourceValue(resource, reader, resourceTypeNames);
						break;

					default:
						this.LoadResourceValue(resource, reader);
						break;
				}

				this.resources[i] = resource;
			}
		}

		public Resource[] Resources
		{
			get { return resources; }
		}

		private int Read7BitEncodedInt(BinaryReader reader)
		{
			int value = 0;
			int shift = 0;
			byte b;

			do
			{
				b = reader.ReadByte();
				value |= (b & 0x7f) << shift;
				shift += 7;
			} 
			while ((b & 0x80) != 0);

			return value;
		}

		// Reads resources saved in 1.0 format.
		private void LoadResourceValue(Resource resource, BinaryReader reader, string[] resourceTypeNames)
		{
			int typeIndex = this.Read7BitEncodedInt(reader);
			if (typeIndex != -1)
			{
				Type type = null;
				try
				{
					type = Type.GetType(resourceTypeNames[typeIndex], true);
				}
				catch (FileNotFoundException exception)
				{
					resource.Exception = exception;
				}
				catch (IndexOutOfRangeException exception)
				{
					resource.Exception = exception;
				}

				if (type == typeof(String))
				{
					resource.Value = reader.ReadString();
				}
				else if (type == typeof(Int32))
				{
					resource.Value = reader.ReadInt32();
				}
				else if (type == typeof(Byte))
				{
					resource.Value = reader.ReadByte();
				}
				else if (type == typeof(SByte))
				{
					resource.Value = reader.ReadSByte();
				}
				else if (type == typeof(Int16))
				{
					resource.Value = reader.ReadInt16();
				}
				else if (type == typeof(Int64))
				{
					resource.Value = reader.ReadInt64();
				}
				else if (type == typeof(UInt16))
				{
					resource.Value = reader.ReadUInt16();
				}
				else if (type == typeof(UInt32))
				{
					resource.Value = reader.ReadUInt32();
				}
				else if (type == typeof(UInt64))
				{
					resource.Value = reader.ReadUInt64();
				}
				else if (type == typeof(Single))
				{
					resource.Value = reader.ReadSingle();
				}
				else if (type == typeof(Double))
				{
					resource.Value = reader.ReadDouble();
				}
				else if (type == typeof(Decimal))
				{
					resource.Value = reader.ReadDecimal();
				}
				else if (type == typeof(DateTime))
				{
					resource.Value = new DateTime(reader.ReadInt64());
				}
				else if (type == typeof(TimeSpan))
				{
					resource.Value = new TimeSpan(reader.ReadInt64());
				}
				else
				{
					this.Deserialize(resource, reader.BaseStream);
				}
			}
		}

		// Reads resources saved in 2.0 format.
		private void LoadResourceValue(Resource resource, BinaryReader reader)
		{
			ResourceTypeCode typeCode = (ResourceTypeCode) this.Read7BitEncodedInt(reader);
			switch (typeCode)
			{
				case ResourceTypeCode.Null:
					resource.Value = null;
					break;

				case ResourceTypeCode.String:
					resource.Value = reader.ReadString();
					break;

				case ResourceTypeCode.Boolean:
					resource.Value = reader.ReadBoolean();
					break;

				case ResourceTypeCode.Char:
					resource.Value = (char) reader.ReadUInt16();
					break;

				case ResourceTypeCode.Byte:
					resource.Value = reader.ReadByte();
					break;

				case ResourceTypeCode.SByte:
					resource.Value = reader.ReadSByte();
					break;

				case ResourceTypeCode.Int16:
					resource.Value = reader.ReadInt16();
					break;

				case ResourceTypeCode.UInt16:
					resource.Value = reader.ReadUInt16();
					break;

				case ResourceTypeCode.Int32:
					resource.Value = reader.ReadInt32();
					break;

				case ResourceTypeCode.UInt32:
					resource.Value = reader.ReadUInt32();
					break;

				case ResourceTypeCode.Int64:
					resource.Value = reader.ReadInt64();
					break;

				case ResourceTypeCode.UInt64:
					resource.Value = reader.ReadUInt64();
					break;

				case ResourceTypeCode.Single:
					resource.Value = reader.ReadSingle();
					break;

				case ResourceTypeCode.Double:
					resource.Value = reader.ReadDouble();
					break;

				case ResourceTypeCode.Decimal:
					resource.Value = reader.ReadDecimal();
					break;

				case ResourceTypeCode.DateTime:
					resource.Value = new DateTime(reader.ReadInt64());
					break;

				case ResourceTypeCode.TimeSpan:
					resource.Value = new TimeSpan(reader.ReadInt64());
					break;

				case ResourceTypeCode.ByteArray:
					resource.Value = reader.ReadBytes(reader.ReadInt32());
					break;

				case ResourceTypeCode.Stream:
                    var value = new MemoryStream(reader.ReadBytes(reader.ReadInt32()));
			        resource.Value = value;
			        resource.Size = value.Length;
					break;

				default:
					this.Deserialize(resource, reader.BaseStream);
					break;
			}
		}

		private void Deserialize(Resource resource, Stream stream)
		{
			try
			{
				BinaryFormatter formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.File | StreamingContextStates.Persistence));
				resource.Value = formatter.Deserialize(stream);
			}
			catch (FileNotFoundException exception)
			{
				resource.Exception = exception;	
				resource.Value = null;
			}
			catch (SerializationException exception)
			{
				resource.Exception = exception;	
				resource.Value = null;
			}
			catch (ArgumentNullException exception)
			{
				resource.Exception = exception;	
				resource.Value = null;
			}
		}

		private enum ResourceTypeCode
		{
			Null = 0,
			String = 1,
			Boolean = 2,
			Char = 3,
			Byte = 4,
			SByte = 5,
			Int16 = 6,
			UInt16 = 7,
			Int32 = 8,
			UInt32 = 9,
			Int64 = 0xa,
			UInt64 = 0xb,
			Single = 0xc,
			Double = 0xd,
			Decimal = 0xe,
			DateTime = 0xf,
			TimeSpan = 0x10,
			ByteArray = 0x20,
			Stream = 0x21,
			StartOfUserTypes = 0x40
		}
	}
}
