#pragma warning disable CS0109
using System.Collections;
using Match3.Data.SocialAlbum;
using SeganX;
using UnityEngine;
using UnityEngine.UI;


public class Popup_SocialAlbumPicturePresenter : GameState
{
    public float canCloseTime;
    public float closeTime;
    
    public Image pictureImage;
    
    public new Animation animation;
    public AnimationClip openPictureAnim;
    public AnimationClip closePictureAnim;

    public AudioSource audioSource;
    public AudioClip openPictureSound;
    public AudioClip closePictureSound;

    private bool isOpened = false;
    public bool isClosed = false;
    private bool canClose = false;
    
    
    public void Setup(SocialAlbumPost post)
    {
        pictureImage.sprite = post.picture.Load();
        pictureImage.SetNativeSize();
        OpenPicture();
    }

    private void OpenPicture()
    {
        if (isOpened) return;
        isOpened = true;
        StartCoroutine(SetCanClose());
        animation.clip = openPictureAnim;
        animation.Play(PlayMode.StopAll);
    }

    private void ClosePicture()
    {
        if (isClosed) return;
        isClosed = true;
        StartCoroutine(AutoClose());
        animation.clip = closePictureAnim;
        animation.Play(PlayMode.StopAll);
    }

    // This methode is calling from animation event
    public void PlayOpenPictureSound()
    {
        audioSource.PlayOneShot(openPictureSound);
    }

    // This methode is calling from animation event
    public void PlayClosePictureSound()
    {
        audioSource.PlayOneShot(closePictureSound);
    }

    private IEnumerator SetCanClose()
    {
        yield return new WaitForSeconds(canCloseTime);
        canClose = true;
    }
    
    public override void Back()
    {
        if (canClose)
            ClosePicture();
    }
    
    private IEnumerator AutoClose()
    {
        yield return new WaitForSeconds(closeTime);
        base.Back();
    }
}
