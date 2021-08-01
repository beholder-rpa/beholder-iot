export interface ObservationRequest {
  adapterIndex?: number;
  deviceIndex?: number;
  regions?: ObservationRegion[];
  streamDesktopThumbnail?: boolean;
  desktopThumbnailStreamSettings?: DesktopThumbnailStreamSettings;
  watchPointerPosition?: boolean;
  streamPointerImage?: boolean;
  pointerImageStreamSettings?: PointerImageStreamSettings;
}

export interface ObservationRegion {
  name: string;
  kind: 'Image' | 'MatrixFrame';
  matrixSettings?: MatrixSettings;
  bitmapSettings?: BitmapSettings;
}

export interface DesktopThumbnailStreamSettings {
  /**
   * Indicates the maximum thumbnails that will be sent per second. Defaults to 0.5
   */
  maxFps?: number;
  /**
   * Indicates the scale factor of the thumbnail. Defaults to 0.15 (15% of original screen size)
   */
  scaleFactor?: number;
}

export interface PointerImageStreamSettings {
  /**
   * Indicates the maximum pointer images that will be sent per second. Defaults to 0.5
   */
  maxFps?: number;
}

export interface MatrixSettings {
  /**
   * An ordered list of points whose values will indicate the data values.
   */
  map?: MatrixPixelLocation[];
  /**
   * Indicates the index of the pixel that will be used as the frame id. (little endian)  (Default: 0)
   */
  frameIdIndex?: number;
  /**
   * Indicates the index of the pixel that will be used for metadata - R - Width, G - Height, B - Pixel Size (Default: 1)
   */
  useFrameMetadata?: number;
  /**
   * Indicates the format that the datamatrix will decode into and ultimately contain (default: Json)
   */
  dataFormat: 'Raw' | 'Hex' | 'Text' | 'TextGrid' | 'Json' | 'MatrixEvents';
}

export interface MatrixPixelLocation {
  index: number;
  x: number;
  y: number;
  width: number;
  height: number;
}

export interface BitmapSettings {
  x: number;
  y: number;
  width: number;
  height: number;
  maxFPS: number;
}
