import { Component, inject, OnInit, signal } from '@angular/core';
import { SessionService } from '../../../core/services/session-service';
import { ActivatedRoute } from '@angular/router';
import { MemberService } from '../../../core/services/member-service';
import { Member, Photo } from '../../../types/member';
import { User } from '../../../types/user';
import { DeleteButton } from "../../../shared/delete-button/delete-button";
import { ImageUpload } from "../../../shared/image-upload/image-upload";

@Component({
  selector: 'app-member-photo',
  imports: [DeleteButton, ImageUpload],
  templateUrl: './member-photo.html',
  styleUrl: './member-photo.css',
})
export class MemberPhoto implements OnInit {

  protected memberService = inject(MemberService);
  private route = inject(ActivatedRoute);
  protected session = inject(SessionService);
  protected photos = signal<Photo[]>([]);// changed to Signal from Observable
  protected loading = signal(false);

  ngOnInit(): void {
    const memberId = this.route.parent?.snapshot.paramMap.get('id');
    if (memberId) {
      this.memberService.getMemberPhotos(memberId).subscribe({
        next: photos => this.photos.set(photos)
      }); // for this to work need to import AsyncPipe
    }
  }

  onUploadImage(file: File) {
    this.loading.set(true);
    this.memberService.uploadPhoto(file).subscribe({
      next: photo => {
        this.memberService.editMode.set(false);
        this.loading.set(false);
        this.photos.update(photos => [...photos, photo]) //... spread operator existing array of photos  and add also the new photo to the array

        if (!this.memberService.member()?.imageUrl) {
          this.setMainLocalPhoto(photo)
        }
      },
      error: error => {
        console.log('Error uploading image: ', error);
        this.loading.set(false);
      }
    })
  }

  setMainPhoto(photo: Photo) {
    this.memberService.setMainPhoto(photo).subscribe({
      next: () => {
        this.setMainLocalPhoto(photo)
      }
    })

  }


  deletePhoto(photoId: number) {
    this.memberService.deletePhoto(photoId).subscribe({
      next: () => {
        this.photos.update(photos => photos.filter(x => x.id !== photoId))
      }
    })

  }

  private setMainLocalPhoto(photo: Photo) {
    const currentUser = this.session.currentUser();
    if (currentUser) currentUser.imageUrl = photo.url;
    this.session.setCurrentUser(currentUser as User);
    this.memberService.member.update(member => ({
      ...member, imageUrl: photo.url
    }) as Member) //From this we'll take the existing properties of the member...member  and we'll set the image URL to be the photo.url 
  }

}
