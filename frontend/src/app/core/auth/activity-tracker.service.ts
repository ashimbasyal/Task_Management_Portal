import { Injectable, inject, NgZone, signal } from '@angular/core';
import { DOCUMENT } from '@angular/common';

@Injectable({ providedIn: 'root' })
export class ActivityTrackerService {
  private document = inject(DOCUMENT);
  private zone = inject(NgZone);
  private lastActivity = signal(Date.now());
  readonly idleThreshold = 5 * 60 * 1000;

  private boundHandler: (() => void) | null = null;

  start() {
    this.boundHandler = () => this.onActivity();
    this.zone.runOutsideAngular(() => {
      const events = ['mousemove', 'keydown', 'click', 'scroll', 'touchstart', 'wheel'];
      for (const ev of events) {
        this.document.addEventListener(ev, this.boundHandler!);
      }
    });
  }

  stop() {
    if (!this.boundHandler) return;
    const events = ['mousemove', 'keydown', 'click', 'scroll', 'touchstart', 'wheel'];
    for (const ev of events) {
      this.document.removeEventListener(ev, this.boundHandler);
    }
    this.boundHandler = null;
  }

  isActive(): boolean {
    return Date.now() - this.lastActivity() < this.idleThreshold;
  }

  private onActivity() {
    this.lastActivity.set(Date.now());
  }
}
