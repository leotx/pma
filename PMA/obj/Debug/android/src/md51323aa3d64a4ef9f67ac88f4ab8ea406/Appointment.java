package md51323aa3d64a4ef9f67ac88f4ab8ea406;


public class Appointment
	extends android.app.Activity
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_onCreate:(Landroid/os/Bundle;)V:GetOnCreate_Landroid_os_Bundle_Handler\n" +
			"";
		mono.android.Runtime.register ("PMA.Appointment, PMA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", Appointment.class, __md_methods);
	}


	public Appointment () throws java.lang.Throwable
	{
		super ();
		if (getClass () == Appointment.class)
			mono.android.TypeManager.Activate ("PMA.Appointment, PMA, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public void onCreate (android.os.Bundle p0)
	{
		n_onCreate (p0);
	}

	private native void n_onCreate (android.os.Bundle p0);

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
