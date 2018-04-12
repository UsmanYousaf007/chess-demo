using UnityEngine;

public class MegacoolReferralCode {

    public string InviteId { get; private set; }

    public string ShareId { get; private set; }

    public MegacoolReferralCode(string inviteId, string shareId) {
        InviteId = inviteId;
        ShareId = shareId;
    }

    public MegacoolReferralCode(AndroidJavaObject jReferralCode) {
        InviteId = jReferralCode.Call<string>("getUserId");
        ShareId = jReferralCode.Call<string>("getShareId");
    }

    public override string ToString() {
        return string.Format("{0}{1}", InviteId, ShareId);
    }
}
