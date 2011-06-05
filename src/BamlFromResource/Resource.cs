namespace Reflector.BamlViewer
{
	using System;

	internal class Resource
	{
		private string name;
		private object value;
		private Exception exception;

		public string Name
		{
			get
			{
				return this.name;
			}
			
			set
			{
				this.name = value;	
			}
		}

		public object Value
		{
			get
			{
				return this.value;
			}
			
			set
			{
				this.value = value;	
			}
		}

		public Exception Exception
		{
			get
			{
				return this.exception;	
			}	
			
			set
			{
				this.exception = value;	
			}
		}
	}
}


