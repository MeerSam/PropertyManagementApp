import { AfterViewInit, Component, ElementRef, input, model, OnInit, output, signal, ViewChild } from '@angular/core';

@Component({
  selector: 'app-file-upload',
  imports: [],
  templateUrl: './file-upload.html',
  styleUrl: './file-upload.css',
})
export class FileUpload implements AfterViewInit {
  @ViewChild('fileInput') fileInputRef!: ElementRef<HTMLInputElement>;
  uploadFile = output<File>(); // output prop that notifies the parent (document Upload) component 
  loading = input<boolean>(false); //input : when api is being notified from parent (document upload) component \.
  protected isDragging = signal<boolean>(false);
  private fileToUpload: File | null = null;
  protected imageSrc = signal<string | ArrayBuffer | null | undefined>(null);
  protected fileName = signal<string | null | undefined>(null);

  ngAfterViewInit(): void {
    console.log(" ngAfterViewInit of the fileUploader Component ", this.fileName())
    this.reset()
  } 

  onDragOver(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();// this to ensure we only want angular to be handling this not the normal event behavior of our browser.
    this.isDragging.set(true);

  }
  onDragLeave(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();// this to ensure we only want angular to be handling this not the normal event behavior of our browser.
    this.isDragging.set(false);
  }

  onDrop(event: DragEvent) {
    event.preventDefault();
    event.stopPropagation();// this to ensure we only want angular to be handling this not the normal event behavior of our browser.
    this.isDragging.set(false);

    if (event.dataTransfer?.files.length) {
      const file = event.dataTransfer.files[0]; //only one doc at a time         
      this.fileToUpload = file;
      this.previewSelectedFile(file);
    }
  }

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];

    if (file) {
      this.fileToUpload = file;
      this.previewSelectedFile(file);
    }
  }

  onCancel() {
    this.reset()
  }

  onUploadFile() {
    if (this.fileToUpload) {
      this.uploadFile.emit(this.fileToUpload);
      this.reset()
    }
  }
  reset() {
    console.log('resetting the file upload component')
    this.fileToUpload = null;
    this.imageSrc.set(null);
    this.fileName.set(null);
    this.isDragging.set(false);
    // Clear the native input so the same file can be re-selected
    if (this.fileInputRef?.nativeElement) {
      this.fileInputRef.nativeElement.value = '';
    }
    console.log('file upload reset')
  }

  private previewSelectedFile(file: File) {
    // Check if file type starts with 'image/'
    if (file.type && !file.type.startsWith('image/')) {
      // Non-image file: display the name
      this.fileName.set(file.name);
      this.imageSrc.set(null); // Clear previous image
      // console.log('Non-image file selected:', file.name);
    } else {
      this.previewImage(file);
    }
  }
  private previewImage(file: File) {
    const reader = new FileReader();
    reader.onload = (e) => this.imageSrc.set(e.target?.result);
    reader.readAsDataURL(file);
  }
}
