import { Component, input, output, signal } from '@angular/core';


@Component({
  selector: 'app-image-upload',
  imports: [],
  templateUrl: './image-upload.html',
  styleUrl: './image-upload.css',
})
export class ImageUpload {
// protected imageSrc? :string |ArrayBuffer |null = null;
  // we have to consider the zoneless angular so make it a signal
  protected imageSrc = signal<string | ArrayBuffer | null | undefined>(null);
  protected isDragging = false;
  private fileToUpload: File | null = null;
  uploadFile = output<File>(); // output prop that notifies the parent (member photo) component 
  loading = input<boolean>(false); //input : if we're loading from parent (member photo) component \.

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();// this to ensure we only want angular to be handling this not the normal event behavior of our browser.
    this.isDragging = true;

  }
  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();// this to ensure we only want angular to be handling this not the normal event behavior of our browser.
    this.isDragging = false;
  }
  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();// this to ensure we only want angular to be handling this not the normal event behavior of our browser.
    this.isDragging = false;

    if (event.dataTransfer?.files.length) {
      const file = event.dataTransfer.files[0]; //only one image at a time
      this.previewImage(file);
      this.fileToUpload = file;
    }
  }

  onCancel() {
    this.fileToUpload = null;
    this.imageSrc.set(null);
  }

  onUploadFile() {
    if (this.fileToUpload) {
      this.uploadFile.emit(this.fileToUpload);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (file) {
      this.fileToUpload = file;
    }
  }


  private previewImage(file: File) {
    const reader = new FileReader();
    reader.onload = (e) => this.imageSrc.set(e.target?.result);
    reader.readAsDataURL(file);
  }

}
