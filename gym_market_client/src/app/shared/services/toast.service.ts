import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

export interface Toast { message: string; type: 'success' | 'error'; id: number; }

@Injectable({ providedIn: 'root' })
export class ToastService {
  private _toasts$ = new BehaviorSubject<Toast[]>([]);
  toasts$ = this._toasts$.asObservable();

  show(message: string, type: 'success' | 'error' = 'success') {
    const id = Date.now();
    this._toasts$.next([...this._toasts$.value, { message, type, id }]);
    setTimeout(() => this.dismiss(id), 3500);
  }

  dismiss(id: number) {
    this._toasts$.next(this._toasts$.value.filter(t => t.id !== id));
  }
}
