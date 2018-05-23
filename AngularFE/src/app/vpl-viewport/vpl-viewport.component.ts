import { Component, OnInit, ViewChild, ElementRef, HostListener, NgZone } from '@angular/core';

@Component({
  selector: 'app-vpl-viewport',
  templateUrl: './vpl-viewport.component.html',
  styleUrls: ['./vpl-viewport.component.scss']
})
export class VplViewportComponent implements OnInit {

  constructor(private _ngZone: NgZone) { }

  @ViewChild('canvas')
  set elem(elem: ElementRef<HTMLCanvasElement>) {
    this.canvasElem = elem.nativeElement;
    this.ctx = this.canvasElem.getContext("2d");
  }

  @HostListener('window:resize')
  private resize() {
    this.canvasElem.width = this.canvasElem.parentElement.parentElement.clientWidth;
    this.canvasElem.height = this.canvasElem.parentElement.parentElement.clientHeight;
    this.draw();
  }

  private ctx: CanvasRenderingContext2D;
  private canvasElem: HTMLCanvasElement;
  camera: Camera = {
    x: 0,
    y: 0,
    scale: 2
  };

  private nodes: Node[] = [
    new Node(5, 5),
    new Node(110, 5)
  ];

  private middleClickDragListener;
  private middleClickMouseUpListener;
  private middleClickStartX;
  private middleClickStartY;

  ngOnInit() {
    this.canvasElem.addEventListener('mousedown', (event: MouseEvent) => { console.log('mousedown', event.button); event.preventDefault(); });
    this.canvasElem.addEventListener('mouseup', (event: MouseEvent) => { console.log('mouseup', event.button); event.preventDefault(); });
    this.canvasElem.addEventListener('mousemove', (event: MouseEvent) => { console.log('mousemove', event.button); event.preventDefault(); });
    //initialize events on canvas here rather than html
    // this.canvasElem.onmousedown = (event: MouseEvent): boolean => {
    //   console.log(event);
    //   switch(event.button) {
    //     case 0: //left click
    //       break;
    //     case 1: //middle click
    //       this.middleClickStartX = event.x;
    //       this.middleClickStartY = event.y;
    //       // this.middleClickDragListener = document.addEventListener('mousemove', (event: MouseEvent) => {
    //       //   console.log('drag');
    //       //   console.log(event.button, event.offsetX, event.offsetY);
    //       //   this.camera.x += (event.x - this.middleClickStartX) / this.camera.scale;
    //       //   this.camera.y += (event.y - this.middleClickStartY) / this.camera.scale;
    //       //   this.draw();
    //       // });
    //       // this.middleClickMouseUpListener = document.addEventListener('mouseup', (event: MouseEvent) => {
    //       //   if(event.button == 1) {
    //       //     document.removeEventListener('mousemove', this.middleClickDragListener);
    //       //     document.removeEventListener('mouseup', this.middleClickMouseUpListener);
    //       //   }
    //       // });
    //       event.preventDefault();
    //       return false;
    //     case 2: //right click
    //       break;
    //   }
    //   return true;
    // }
    this.resize();
  }

  private draw() {
    let cameraBox = new Box();
    cameraBox.x = this.camera.x,
    cameraBox.y = this.camera.y;
    cameraBox.height = this.canvasElem.height / this.camera.scale;
    cameraBox.width = this.canvasElem.width / this.camera.scale;

    console.log('draw');
    this.ctx.clearRect(0, 0, this.canvasElem.width, this.canvasElem.height);
    this.ctx.fillStyle = 'red';
    this.ctx.fillRect(0, 0, this.canvasElem.width, this.canvasElem.height);
    this.nodes.forEach((n,i) => {
      if(cameraBox.intersects(n.box)) {
        console.log(i);
        this.drawNode(n);
      }
    });
  }

  private drawNode(node: Node) {
    const r = 10;
    let
      x = (node.box.x - this.camera.x) * this.camera.scale,
      y = (node.box.y - this.camera.y) * this.camera.scale,
      w = node.box.width * this.camera.scale,
      h = node.box.height * this.camera.scale;
    
    this.ctx.save();
    this.ctx.fillStyle = 'white';
    this.ctx.fillRect(x, y, w, h);
    this.ctx.fillStyle = 'black';
    this.ctx.strokeRect(x, y, w, h);
    this.ctx.restore();
  }
}

class Box {
  x: number;
  y: number;
  width: number;
  height: number;

  intersects(box: Box): boolean {
    return (Math.abs(this.x - box.x) * 2 < (this.width + box.width)) &&
      (Math.abs(this.y - box.y) * 2 < (this.height + box.height));
  }
}

class Node {
  box: Box;

  constructor(x: number, y: number) {
    this.box = new Box();
    this.box.x = x;
    this.box.y = y;
    this.box.width = 100;
    this.box.height = 100;
  }
}

class Camera {
  x: number;
  y: number;
  scale: number;
}
