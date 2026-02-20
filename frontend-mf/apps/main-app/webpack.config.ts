import { withModuleFederation } from '@nx/module-federation/angular';
import config from './module-federation.config';

/**
 * DTS Plugin is disabled in Nx Workspaces as Nx already provides Typing support for Module Federation
 * The DTS Plugin can be enabled by setting dts: true
 * Learn more about the DTS Plugin here: https://module-federation.io/configure/dts.html
 */
export default async (env?: NodeJS.ProcessEnv, argv?: Record<string, string>) => {
  const mfExecutor = await withModuleFederation(config, { dts: false });
  const mfConfig = await mfExecutor(env, argv);
  return {
    ...mfConfig,
    output: {
      ...mfConfig.output,
      // Explicit publicPath prevents the MF runtime from using
      // import.meta.url (ESM-only) to resolve chunk URLs, which causes
      // "import.meta may only appear in a module" in browser scripts.
      publicPath: 'http://localhost:4200/',
    },
    // 'window' type tells webpack to resolve the external as window['signalR']
    // rather than generating an ESM import specifier (which the browser rejects
    // as a bare specifier when chunks run as regular scripts, not ES modules).
    externalsType: 'window',
    externals: {
      ...(mfConfig.externals as Record<string, string> ?? {}),
      '@microsoft/signalr': 'signalR',
    },
  };
};
