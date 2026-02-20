import { ModuleFederationConfig } from '@nx/module-federation';

const config: ModuleFederationConfig = {
  name: 'main-app',
  remotes: [],
  // Exclude @microsoft/signalr from shared deps so it is bundled into the app.
  // When shared, its chunk fails to load (ChunkLoadError) due to MF resolution.
  shared: (libraryName) => (libraryName === '@microsoft/signalr' ? false : undefined),
};

export default config;
