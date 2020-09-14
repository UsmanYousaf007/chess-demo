using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TurboLabz.InstantFramework;

public class ES2UserType_TurboLabzInstantFrameworkNotification : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		TurboLabz.InstantFramework.Notification data = (TurboLabz.InstantFramework.Notification)obj;
		// Add your writer.Write calls here.
		writer.Write(data.title);
		writer.Write(data.body);
		writer.Write(data.timestamp);
		writer.Write(data.sender);

	}
	
	public override object Read(ES2Reader reader)
	{
		TurboLabz.InstantFramework.Notification data = new TurboLabz.InstantFramework.Notification();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		TurboLabz.InstantFramework.Notification data = (TurboLabz.InstantFramework.Notification)c;
		// Add your reader.Read calls here to read the data into the object.
		data.title = reader.Read<System.String>();
		data.body = reader.Read<System.String>();
		data.timestamp = reader.Read<System.Int64>();
		data.sender = reader.Read<System.String>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_TurboLabzInstantFrameworkNotification():base(typeof(TurboLabz.InstantFramework.Notification)){}
}