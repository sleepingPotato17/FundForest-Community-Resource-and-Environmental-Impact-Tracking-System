import tkinter as tk
from tkinter import ttk

from db.database import Database
from utils.theme import apply_theme
from ui import dashboard, beneficiaries, programs, donors, donations, distributions


def main():
    """Main application entry point"""
    
    # Initialize database connection
    db = Database()
    
    # Create main window
    root = tk.Tk()
    root.title("🌿 FundForest — Community Support Tracker")
    root.geometry("1400x800")
    try:
        root.state('zoomed')  # Maximize window on Windows
    except:
        pass  # Skip if not supported on other OS
    
    # Apply custom theme
    apply_theme(root)
    
    # ===== SIDEBAR =====
    sidebar = ttk.Frame(root, style="Sidebar.TFrame")
    sidebar.pack(side="left", fill="y")
    
    # Logo/Header area
    logo_frame = ttk.Frame(sidebar, style="Sidebar.TFrame")
    logo_frame.pack(fill="x", pady=30, padx=20)
    
    ttk.Label(logo_frame, text="🌿 FUNDFOREST", 
             style="Logo.TLabel").pack()
    ttk.Label(logo_frame, text="Community Support Tracker", 
             style="Subtitle.TLabel").pack(pady=(5, 0))
    
    # Navigation buttons container
    nav_container = ttk.Frame(sidebar, style="Sidebar.TFrame")
    nav_container.pack(fill="both", expand=True, pady=20)
    
    # ===== MAIN CONTENT CONTAINER =====
    container = ttk.Frame(root, style="Container.TFrame")
    container.pack(side="right", fill="both", expand=True)
    
    # Page definitions
    pages = {
        "Dashboard": dashboard.create_page,
        "Beneficiaries": beneficiaries.create_page,
        "Programs": programs.create_page,
        "Donors": donors.create_page,
        "Donations": donations.create_page,
        "Distributions": distributions.create_page
    }
    
    # Track current page
    current = {"frame": None, "active": "Dashboard"}
    
    def switch_page(page_name):
        """Switch between different pages"""
        if current["frame"]:
            current["frame"].destroy()
        
        current["frame"] = pages[page_name](container, db)
        current["frame"].pack(fill="both", expand=True, padx=30, pady=30)
        current["active"] = page_name
        update_nav_buttons()
    
    def update_nav_buttons():
        """Update navigation button styles"""
        # Clear only the navigation container
        for widget in nav_container.winfo_children():
            widget.destroy()
        
        icons = {
            "Dashboard": "📊",
            "Beneficiaries": "👥",
            "Programs": "📋",
            "Donors": "🤝",
            "Donations": "💰",
            "Distributions": "📦"
        }
        
        for name in pages:
            is_active = (name == current["active"])
            style = "ActiveNav.TButton" if is_active else "Nav.TButton"
            icon = icons.get(name, "•")
            
            btn = ttk.Button(nav_container, 
                           text=f"{icon}  {name}", 
                           command=lambda n=name: switch_page(n), 
                           style=style)
            btn.pack(fill="x", pady=6, padx=20)
    
    # Version info at bottom
    version_frame = ttk.Frame(sidebar, style="Sidebar.TFrame")
    version_frame.pack(side="bottom", pady=20)
    ttk.Label(version_frame, text="v2.0 | MariaDB Edition", 
             style="Subtitle.TLabel").pack()
    
    update_nav_buttons()
    switch_page("Dashboard")
    root.mainloop()


if __name__ == "__main__":
    print("=" * 70)
    print("🌿 FundForest - Community Support Tracker")
    print("=" * 70)
    print("✓ Make sure XAMPP with MariaDB is running")
    print("=" * 70)
    main()