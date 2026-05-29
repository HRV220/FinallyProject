import io
import contextlib
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt
import matplotlib.ticker as mticker
import numpy as np
from fastapi import FastAPI
from fastapi.responses import Response
from pydantic import BaseModel
from typing import List


@contextlib.asynccontextmanager
async def lifespan(_: "FastAPI"):
    # Pre-render a dummy chart so matplotlib builds its font cache before
    # the first real request arrives (cold init can take 30–60 s otherwise).
    fig, ax = plt.subplots()
    buf = io.BytesIO()
    fig.savefig(buf, format="png", dpi=72)
    plt.close(fig)
    yield


app = FastAPI(title="FinanceTracker Chart Service", lifespan=lifespan)

# ─── Theme (matches frontend palette) ────────────────────────────────────────
C_INCOME  = "#10b981"
C_EXPENSE = "#ef4444"
C_BALANCE = "#3b82f6"
C_GRID    = "#e5e7eb"
C_BG      = "#ffffff"
C_TEXT    = "#374151"
C_ZERO    = "#9ca3af"

plt.rcParams.update({
    "font.family":        ["DejaVu Sans", "sans-serif"],
    "axes.facecolor":     C_BG,
    "figure.facecolor":   C_BG,
    "axes.edgecolor":     "#d1d5db",
    "axes.grid":          True,
    "grid.color":         C_GRID,
    "grid.linestyle":     "--",
    "grid.alpha":         0.7,
    "axes.spines.top":    False,
    "axes.spines.right":  False,
    "text.color":         C_TEXT,
    "axes.labelcolor":    C_TEXT,
    "xtick.color":        C_TEXT,
    "ytick.color":        C_TEXT,
    "xtick.labelsize":    9,
    "ytick.labelsize":    9,
})

MONTHS = ["янв", "фев", "мар", "апр", "май", "июн",
          "июл", "авг", "сен", "окт", "ноя", "дек"]


# ─── Models ───────────────────────────────────────────────────────────────────

class MonthlyBarRequest(BaseModel):
    labels: List[str]    # ["2025-01", ...]
    income: List[float]
    expense: List[float]


class BalanceLineRequest(BaseModel):
    labels: List[str]
    balance: List[float]


class CategoryItem(BaseModel):
    name: str
    value: float
    color: str


class CategoryPieRequest(BaseModel):
    items: List[CategoryItem]
    title: str = "Структура"


# ─── Helpers ──────────────────────────────────────────────────────────────────

def fmt_month(label: str) -> str:
    try:
        parts = label.split("-")
        return f"{MONTHS[int(parts[1]) - 1]}'{parts[0][2:]}"
    except Exception:
        return label


def amount_fmt(x, _):
    if abs(x) >= 1_000_000:
        return f"{x / 1_000_000:.1f}М"
    if abs(x) >= 1_000:
        return f"{x / 1_000:.0f}К"
    return f"{x:.0f}"


def to_png(fig: plt.Figure) -> bytes:
    buf = io.BytesIO()
    fig.savefig(buf, format="png", dpi=100, bbox_inches="tight", facecolor=C_BG)
    buf.seek(0)
    plt.close(fig)
    return buf.read()


# ─── Endpoints ────────────────────────────────────────────────────────────────

@app.get("/health")
def health():
    return {"status": "ok"}


@app.post("/charts/monthly-bar")
def monthly_bar(req: MonthlyBarRequest):
    if not req.labels:
        fig, ax = plt.subplots(figsize=(10, 4.5))
        ax.text(0.5, 0.5, "Нет данных", ha="center", va="center",
                transform=ax.transAxes, fontsize=14, color=C_ZERO)
        ax.axis("off")
        return Response(content=to_png(fig), media_type="image/png")

    labels = [fmt_month(l) for l in req.labels]
    n = len(labels)
    x = np.arange(n)
    w = min(0.38, 0.9 / 2)

    fig, ax = plt.subplots(figsize=(max(10, n * 0.8), 4.5))
    ax.bar(x - w / 2, req.income,  w, label="Доход",  color=C_INCOME,  alpha=0.9)
    ax.bar(x + w / 2, req.expense, w, label="Расход", color=C_EXPENSE, alpha=0.9)
    ax.set_xticks(x)
    ax.set_xticklabels(labels)
    ax.yaxis.set_major_formatter(mticker.FuncFormatter(amount_fmt))
    ax.set_title("Доходы и расходы по месяцам", fontsize=13, fontweight="bold", pad=10)
    ax.legend(framealpha=0, fontsize=10)
    fig.tight_layout()
    return Response(content=to_png(fig), media_type="image/png")


@app.post("/charts/balance-line")
def balance_line(req: BalanceLineRequest):
    if not req.labels:
        fig, ax = plt.subplots(figsize=(10, 4.5))
        ax.text(0.5, 0.5, "Нет данных", ha="center", va="center",
                transform=ax.transAxes, fontsize=14, color=C_ZERO)
        ax.axis("off")
        return Response(content=to_png(fig), media_type="image/png")

    labels = [fmt_month(l) for l in req.labels]
    n = len(labels)
    x = np.arange(n)

    fig, ax = plt.subplots(figsize=(max(10, n * 0.8), 4.5))
    ax.plot(x, req.balance, color=C_BALANCE, linewidth=2.5, marker="o", markersize=5)
    ax.fill_between(x, req.balance, alpha=0.10, color=C_BALANCE)
    ax.axhline(0, color=C_ZERO, linewidth=0.8, linestyle=":")
    ax.set_xticks(x)
    ax.set_xticklabels(labels)
    ax.yaxis.set_major_formatter(mticker.FuncFormatter(amount_fmt))
    ax.set_title("Динамика баланса", fontsize=13, fontweight="bold", pad=10)
    fig.tight_layout()
    return Response(content=to_png(fig), media_type="image/png")


@app.post("/charts/category-pie")
def category_pie(req: CategoryPieRequest):
    if not req.items:
        fig, ax = plt.subplots(figsize=(8, 6))
        ax.text(0.5, 0.5, "Нет данных", ha="center", va="center",
                transform=ax.transAxes, fontsize=14, color=C_ZERO)
        ax.axis("off")
        return Response(content=to_png(fig), media_type="image/png")

    names  = [i.name for i in req.items]
    values = [i.value for i in req.items]
    colors = [i.color for i in req.items]
    total  = sum(values)

    fig, ax = plt.subplots(figsize=(9, 6.5))
    wedges, _, autotexts = ax.pie(
        values,
        labels=None,
        colors=colors,
        autopct=lambda p: f"{p:.1f}%" if p > 4 else "",
        startangle=140,
        pctdistance=0.78,
        wedgeprops={"linewidth": 0.8, "edgecolor": "white"},
    )
    for at in autotexts:
        at.set_fontsize(8)
        at.set_color("white")

    legend_labels = [
        f"{n}  —  {v:,.0f} ₽  ({v / total * 100:.1f}%)"
        for n, v in zip(names, values)
    ]
    ax.legend(
        wedges, legend_labels,
        loc="lower center", bbox_to_anchor=(0.5, -0.25),
        ncol=2, fontsize=8, framealpha=0,
    )
    ax.set_title(req.title, fontsize=13, fontweight="bold", pad=10)
    fig.tight_layout(rect=[0, 0.08, 1, 1])
    return Response(content=to_png(fig), media_type="image/png")
