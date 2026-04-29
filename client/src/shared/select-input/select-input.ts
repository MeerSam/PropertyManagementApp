import { Component, forwardRef, input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NG_VALUE_ACCESSOR, NgControl, ReactiveFormsModule, Validator } from '@angular/forms';
import { SelectOption } from '../../types/select';
import { disabled } from '@angular/forms/signals';

@Component({
  selector: 'app-select-input',
  imports: [ReactiveFormsModule],
  templateUrl: './select-input.html',
  styleUrl: './select-input.css',
  // providers: [
  //   {
  //     provide: NG_VALUE_ACCESSOR,
  //     useExisting: forwardRef(() => SelectInput),
  //     multi: true
  //   }
  // ]
})
export class SelectInput implements ControlValueAccessor {

  label = input<string>('');
  placeholder = input<string>('Select an option');
  options = input<SelectOption[]>([]);
  // control = input<FormControl>(new FormControl());
  private onTouched = () => { };
  private onChange = (_: string) => { };

  constructor(@Self() public ngControl: NgControl) {
    // Select input is a type of Ng control, and we're assigning the select(this) to the ngcontrol value accessor.
    //@Self() -  use a decorator of self that we get from angular core  And what does this mean. 
    // So this is what's referred to as a dependency injection modifier.
    // @Self() tells angular to only look for this dependency that we're injecting on the current elements, 
    // and not to search up the injector hierarchy, the tree in its parents or its ancestors,
    this.ngControl.valueAccessor = this;
    //“Use this component as the ControlValueAccessor for the form control.”
  }


  get control(): FormControl {
    // //Expose the real form control for validation in the template 
    return this.ngControl.control as FormControl;
  } 

  onSelect(event: Event) {
    const val = (event.target as HTMLSelectElement).value;
    this.control.setValue(val);
    this.control.markAsTouched();
    this.onChange(val);
    this.onTouched();
  }

  //Angular calls this when the form wants to push a value into your component.
  writeValue(val: string): void {
    // this.control.setValue(val ?? '', { emitEvent: false });
    // Nothing to do — [value] binding on <select> handles display
  }

  //Angular gives you a callback to call whenever the user changes the value.
  registerOnChange(fn: (_: string) => void) { this.onChange = fn; }

  // Angular gives you a callback to call when the user touches the control.
  registerOnTouched(fn: () => void) { this.onTouched = fn; }

  // Angular tells you when the control should be disabled.
  setDisabledState?(isDisabled: boolean): void {
     if (!this.control) return; // ← guard for early calls before ngOnInit
    isDisabled ? this.control?.disable() : this.control.enable();
  }
}
