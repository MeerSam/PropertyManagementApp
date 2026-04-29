import { Component, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl, ReactiveFormsModule } from '@angular/forms';


@Component({
  selector: 'app-text-input',
  imports: [ReactiveFormsModule],
  templateUrl: './text-input.html',
  styleUrl: './text-input.css',
})

export class TextInput implements ControlValueAccessor{
  // input properties 

  label = input<string>('');
  type = input<string>('text');
  maxDate = input<string>(''); 
  
  constructor(@Self() public ngControl:NgControl){
    // text input is a type of Ng control, and we're assigning the text input (this) to the ngcontrol value accessor.
    //@Self() -  use a decorator of self that we get from angular core and add the parentheses. And what does this mean. 
    // So this is what's referred to as a dependency injection modifier.
    // going to tell angular to only look for this dependency that we're injecting on the current
    // elements, and not to search up the injector hierarchy, the tree in its parents or its ancestors,
      this.ngControl.valueAccessor = this;
  }

  get control(): FormControl {
    return this.ngControl.control as FormControl;
  }


  writeValue(obj: any): void { 
  }
  registerOnChange(fn: any): void { 
  }
  registerOnTouched(fn: any): void { 
  }  
}
